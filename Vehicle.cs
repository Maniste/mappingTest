using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField]
    private float acceleration = 0.1f;
    [SerializeField]
    private float turnSpeed = 0.1f;
    [SerializeField]
    private float maxSpeed = 5f;
    [SerializeField]
    private float gravity = 5f;
    [SerializeField]
    private float jumpVelocity = 15f;
    [SerializeField]
    private float jumpDeacceleration = 15f;

    [SerializeField]
    private float lossOfSpeed = 5f;

    private Vector3 velocity = Vector3.zero;
    private float _vel = 0f;
    private float jumpForce = 0f;
    private float currentGravity = 0;
    private bool jumping = false;
    private bool grounded = false;
    private bool restarting = false;

    private bool onHalfPipe = false;
    private Vector3 currentGroundNormal = Vector3.zero;

    private Transform _meshRoot = null;

    [SerializeField] private float failTimer = 1f;
    private float curFailTime = 0f;

    [SerializeField] private AudioClip _horseFall = null;
    private AudioSource _as = null;
    void Awake()
    {
        //the camera point is at index 0
        _meshRoot = transform.GetChild(1).transform;
        _as = GetComponent<AudioSource>();
        currentGravity = gravity;
    }

    void Start()
    {

    }

    private void PlayAudio(AudioClip clip)
    {
        _as.clip = clip;
        _as.Play();
    }

    private void AligenToGround()
    {
        Vector3 TL = new Vector3(0f,0.3f,0f);

        Vector3 newNormal = Vector3.zero;
        Vector3 newPos = transform.position;

        float yangle = _meshRoot.localEulerAngles.x;
        float yangle2 = _meshRoot.localEulerAngles.y;
        //Debug.Log("| X: " + yangle + " | y: " + yangle2);
        if (Physics.Raycast(transform.position + TL, -Vector3.up, out RaycastHit hit1, 1f, 1))
        {
            //if we are jumping then
            //set grounded to false
            if (jumping)
            {
                //if we arent touching the ground yet then
                //dont aligne to it
                if (hit1.distance > 0.1f)
                {
                    grounded = false;
                    return;
                }
            }

            //we just landed after being airborne, are we turned to the right direction?
            if (grounded == false)
            {
                bool failedLand = true;
                //check if our angle on the x axies is within 20
                //if not then we fail the landing
                //and then do the same for the Y axis
                //but this time its wether or not
                //we point to the same direction our velocity is towards
                //or completly the opposite
                Vector3 rot = _meshRoot.localEulerAngles;
                if (rot.x < 35f || rot.x > 325f)
                    if (rot.y > 320f || rot.y < 20f || rot.y > 140f || rot.y < 200f)
                        failedLand = false;

                if (failedLand)
                {
                    //Reset character
                    restarting = true;
                    curFailTime = 0;

                    PlayAudio(_horseFall);
                    Debug.Log("horse fell");


                    _vel = 0;
                    jumping = false;
                    jumpForce = 0;

                    musicHandler.OnStopMusic?.Invoke();

                    grounded = true;
                    return;
                }
                else
                {
                    transform.rotation = _meshRoot.rotation;
                    _meshRoot.localRotation = Quaternion.identity;
                }
            }

            if (onHalfPipe)
                if (hit1.normal.y == 1f)
                    return;

            currentGroundNormal = hit1.normal;
            newNormal = hit1.normal;
            newPos.y = hit1.point.y + 0.5f;
            grounded = true;
        }
        else
        {
            currentGroundNormal = Vector3.zero;
            grounded = false;
        }

        Debug.DrawRay(transform.position + TL, -transform.up * 0.5f, Color.red);

        Quaternion newrot = transform.localRotation;

        if (newNormal != Vector3.zero)
        {
            Quaternion nR = Quaternion.FromToRotation(newNormal, Vector3.up);
            nR.z = -nR.z;
            nR.x = -nR.x;

            newrot.x = 0f;
            newrot.z = 0f;
            Quaternion yOnly = newrot;

            newrot = nR * yOnly;
        }

        transform.localRotation = newrot;
        transform.position = newPos;
    }

    private void GroundMovement()
    {
        //Rotate the object based on the turn speed
        if (Input.GetAxisRaw("Horizontal") != 0f)
        {
            float dir = Input.GetAxisRaw("Horizontal") * turnSpeed;
            Quaternion newRot = transform.rotation;
            newRot.y += (dir * turnSpeed) * Time.deltaTime;
            transform.Rotate(Vector3.up * dir * Time.deltaTime, Space.Self);
        }


        //check the direction we are going towards
        //if its moving downwards then completly ignore
        //this part
        //eles if we are rolling upwards(hill, ramp)
        //increase the loss of speed value
        //and make us go backwards

        float multi = 1;    //modify our speed when moving
        float xDir = transform.rotation.eulerAngles.x;
        //we are going up hill
        if (xDir > 280f)
        {
            multi = 0.1f;
            Debug.Log("Going up");

            if (_vel > maxSpeed * 0.5f)
                _vel = maxSpeed * 0.5f;

        }//we are going down hill
        else if (xDir > 0f && xDir < 30f)
        {
            Debug.Log("Going down");
            multi = 10f;

        }

 

        //Add friction value onto the velocity
        //depending on the button we are pressing down
        if (Input.GetAxisRaw("Vertical") != 0f)
        {
            float dir = Input.GetAxisRaw("Vertical");
            _vel += (acceleration * multi) * dir * Time.deltaTime;
        }
        else
        {
            //Slow down the object over time
            //based on the lossOfSpeed value
            //also make sure the same happens if
            //we go in reverse

            float mod = 1f;

            //only check if the multiplier
            //has been modified
            if (multi != 1)
                if (multi > 1)//we go down so ignore loss of speed
                    mod = 0f;
                else if (multi < 1)//we are going up so make it stronger
                    mod = 1.3f;

            if (_vel > 0.05)
                _vel -= (lossOfSpeed * mod) * Time.deltaTime;
            else if (_vel < 0.05)
                _vel += (lossOfSpeed * mod) * Time.deltaTime;
            else
                _vel = 0f;
        }


        //Make sure to limit the velocity
        //if we go over the max speed
        //also make sure we cant go above a certain speed
        //if we reverse
        if (_vel > maxSpeed)
            _vel = maxSpeed;
        else if (_vel < -25f)
            _vel = -25f;
    }

    private void AirMovement()
    {
        Quaternion oldRot = _meshRoot.localRotation;
        Quaternion newRot = _meshRoot.localRotation;
        float x = 0f, y = 0f;
        float tempSpeed = turnSpeed * (1f + (_vel * 0.05f));

        //spin the board 180 degrees
        if (Input.GetButtonDown("Spin"))
        { 
        
        
        }

        //Rotate the object based on the turn speed
        if (Input.GetButton("Trick"))
        {
            if (Input.GetAxisRaw("Horizontal") != 0f)
            {
                float dir = Input.GetAxisRaw("Horizontal");
                y += (dir * tempSpeed) * Time.deltaTime;
            }

            if (Input.GetAxisRaw("Vertical") != 0f)
            {
                float dir = Input.GetAxisRaw("Vertical");
                x += (dir * tempSpeed) * Time.deltaTime;
            }
        }

        _meshRoot.localRotation = newRot * Quaternion.Euler(x, y, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (restarting)
        {
            if (curFailTime >= failTimer)
            {
                //reset rotational value
                _meshRoot.localRotation = Quaternion.identity;
                musicHandler.OnStartMusic?.Invoke();
                restarting = false;
            }
            curFailTime += Time.deltaTime;
            return;
        }


        if (grounded)
            GroundMovement();
        else
            AirMovement();

        //we dont want to aligne to the ground if we are jumping
        AligenToGround();

        if (Input.GetButtonDown("Jump"))
            if (grounded)
                if (!jumping)
                {
                    currentGravity = 0f;
                    jumpForce = jumpVelocity;
                    jumping = true;
                }

        if (grounded)
        {
            if (_vel < 0.08)
                _vel = 0f;

            if (onHalfPipe)
            {
                Vector3 proj = Vector3.Project(transform.position - currentGroundNormal, currentGroundNormal);
                transform.position +=
                    (transform.forward * _vel * Time.deltaTime) +
                    (transform.right * 0.05f * Input.GetAxisRaw("Horizontal") * Time.deltaTime) +
                    (Vector3.up * jumpForce * Time.deltaTime);
            }
            else
            {
                //Finally add the current velocity onto the objects transform
                  transform.position +=
                    (transform.forward * _vel * Time.deltaTime) +
                    (transform.right * 0.05f * Input.GetAxisRaw("Horizontal") * Time.deltaTime) +
                    (Vector3.up * jumpForce * Time.deltaTime);
            }
        }
        else
        {
            transform.position += (transform.forward * _vel * Time.deltaTime) +
                (Vector3.up * jumpForce * Time.deltaTime);
        }

        if (!grounded && !jumping)
        {
            currentGravity = Mathf.Lerp(currentGravity, gravity, Time.deltaTime * 2f);
            if (currentGravity > gravity * 0.79f)
                currentGravity = gravity;

            transform.position += currentGravity * Time.deltaTime * -Vector3.up;
        }
        if (jumping)
        {
            jumpForce -= jumpDeacceleration * Time.deltaTime;
            if (jumpForce < 0)
            {
                jumping = false;
                jumpForce = 0;
            }
        }

        velocity = transform.position.normalized * _vel;
    }
}
