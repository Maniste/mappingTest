using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[System.Serializable]
public class TileMaker : MonoBehaviour
{
    public GameObject preMadeMesh = null;
    public GameObject breakableMesh = null;
    public GameObject enemyTile = null;

    private Material _floorMaterial = null;
    private Material _wallMaterial = null;

    public void SetTextures(Material newWallTex, Material newFloorTex)
    {
        _wallMaterial = newWallTex;
        _floorMaterial = newFloorTex;
    }

    public void MakeFloor(Vector3 pos, int enemyEncounter = 0)
    {
            Vector3[] verts = { new Vector3(0f, 0f, 0f), new Vector3(0f, 3f, 0f), new Vector3(3f, 3f, 0f), new Vector3(3f, 0f, 0f) };
            Vector2[] uvs = { new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f) };
            int[] tris = { 0, 1, 2, 0, 2, 3 };

            Mesh newMesh = new Mesh();
            newMesh.name = "FloorTile";
            newMesh.vertices = verts;
            newMesh.uv = uvs;
            newMesh.triangles = tris;
            newMesh.RecalculateNormals();
            GameObject tile = new GameObject("Tile", typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider));
            tile.GetComponent<MeshFilter>().mesh = newMesh;

            tile.transform.Rotate(90f, 00f, 0f);

            //fix pos because pivot isnt in center
            pos += Vector3.left * 1.5f + -Vector3.forward * 1.5f;

            tile.transform.position = pos;

            tile.GetComponent<MeshRenderer>().material = _floorMaterial;

        if (enemyEncounter == 1)
        {
            GameObject enemyBattleTile = Instantiate(enemyTile, pos, Quaternion.identity);
        }
    }

    public void MakeWall(Vector3 pos, Vector3 direction, bool breakable = false)
    {
        //  3 - - - - 2
        //  :         :
        //  :         :
        //  :         :
        //  0 - - - - 1

        Quaternion wallRot = Quaternion.identity;

        Vector3[] verts = new Vector3[4];
        Vector2[] uvs = { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f) };
        int[] tris = { 0, 1, 2, 0, 2, 3 };
        Mesh newWall = new Mesh();

        if (direction == Vector3.up)
        {
            pos += Vector3.left * 1.5f + Vector3.forward * 1.5f;

            verts[0] = new Vector3(0f, 0f, 0f);
            verts[1] = new Vector3(3f, 0f, 0f);
            verts[2] = new Vector3(3f, 3f, 0f);
            verts[3] = new Vector3(0f, 3f, 0f);
        }
        else if (direction == -Vector3.up)
        {
            pos += Vector3.left * 1.5f + -Vector3.forward * 1.5f;

            verts[0] = new Vector3(0f, 0f, 0f);
            verts[1] = new Vector3(3f, 0f, 0f);
            verts[2] = new Vector3(3f, 3f, 0f);
            verts[3] = new Vector3(0f, 3f, 0f);
        }
        else if (direction == Vector3.right)
        {
            pos += Vector3.right * 1.5f + -Vector3.forward * 1.5f;

            verts[0] = new Vector3(0f, 0f, 0f);
            verts[1] = new Vector3(0f, 0f, 3f);
            verts[2] = new Vector3(0f, 3f, 3f);
            verts[3] = new Vector3(0f, 3f, 0f);
        }
        else if (direction == -Vector3.right)
        {

            pos += Vector3.left * 1.5f + -Vector3.forward * 1.5f;

            //TODO: INVERT NORMALS
            verts[0] = new Vector3(0f, 0f, 0f);
            verts[1] = new Vector3(0f, 0f, 3f);
            verts[2] = new Vector3(0f, 3f, 3f);
            verts[3] = new Vector3(0f, 3f, 0f);
        }
        
        newWall.name = "WallTile";
        newWall.vertices = verts;
        newWall.uv = uvs;
        newWall.triangles = tris;
        newWall.RecalculateNormals();

        GameObject tile = new GameObject("Wall Tile", typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider));
        tile.GetComponent<MeshFilter>().mesh = newWall;

        tile.transform.position = pos;


        Vector3 scale = tile.transform.localScale;
        //normal band aid fix
        if (direction == -Vector3.right || direction == Vector3.up) 
        {
            if (direction == -Vector3.right)
                scale.x = -scale.x;
            else
                scale.z = -scale.z;

            tile.transform.localScale = scale;
        }


        tile.GetComponent<MeshRenderer>().material = _wallMaterial;
        tile.GetComponent<MeshCollider>().sharedMesh = newWall;

        //Real Tile
        GameObject newMesh;

        //if the wall isnt breakable, use the static wall
        if (breakable == false)
            newMesh = preMadeMesh;
        else
            newMesh = breakableMesh;

        GameObject Realtile = Instantiate(newMesh, pos, Quaternion.identity);
        Vector3 addedDir = Vector3.zero;
        Vector3 scal = Realtile.transform.localScale;
        Quaternion rot =new Quaternion(0f, 0f, 0f, 1f);

      
        if (direction == Vector3.up)
        {
            scal.z = -scal.z;
            Realtile.name = "forward Wall";
        }
        else if (direction == -Vector3.up)
        {
            Realtile.name = "backwards Wall";

            if (breakable)
                scal.z = -scal.z;

        }
        else if (direction == Vector3.right)
        {
            //scal.z = -scal.z;
            rot = Quaternion.Euler(0f, -90f, 0f);
            Realtile.name = "right Wall";
        }
        else if (direction == -Vector3.right)
        {
            addedDir += Vector3.forward * 3;
            rot = Quaternion.Euler(0f, 90f, 0f);
            Realtile.name = "left Wall";
        }
        else
        {
            //scal.z = -scal.z;
            Realtile.name = "left Wall";
        }

        Realtile.transform.position += addedDir;
        Realtile.transform.localScale = scal;
        Realtile.transform.localRotation = rot;

        var flags = StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic;
        GameObjectUtility.SetStaticEditorFlags(Realtile, flags);

        Destroy(tile);
    }
}
