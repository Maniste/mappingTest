using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MapSceneSetUp : MonoBehaviour
{
    public delegate Material[] GetMaterialArray();
    public static GetMaterialArray OnGetMaterial;

    [SerializeField]
    private bool randomMap = false;

    [SerializeField]
    [Range(1f, 100f)]
    private int randTreshold = 50;

    [SerializeField]
    [Range(1, 4)]
    private int smoothing = 1;

    [SerializeField] private string generatorSeed = "";

    [SerializeField] private MapSize size = MapSize.small;

    [SerializeField] private GameObject defaultPlayer = null;
    private GameObject _playerClass = null;

    [SerializeField] private AudioClip[] _defaultMapSong = null;
    private AudioClip _mapSong = null;
    private AudioSource _musicSource = null;

    private Vector3 _playerSpawnPosition = new Vector3(0f, 0.75f, 0f);

    [SerializeField] private string defaultMapJsonName = "";
    private string _mapJsonName = null;

    private Map _mapGenCode;
    private TileMaker _tileMaker;

    [SerializeField]
    private Material[] _materialList = null;
    private Material[] GetMaterialList()
    {
        return _materialList;
    }

    private void Awake()
    {
        _musicSource = GetComponent<AudioSource>();
        _tileMaker = GetComponent<TileMaker>();
        _mapGenCode = new Map();

        //bandaid fix for Map.cs where it calls the GetMaterialList delegate
        //in awake now instead of start, so before OnEnable can run and assign the listener
        OnGetMaterial += GetMaterialList;

        bool canFindMap = false;
        GameObject player = Instantiate(defaultPlayer, _playerSpawnPosition, Quaternion.identity);

        string dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/SMTMAPMAKER";
        if (Directory.Exists(dir))
        {
            string path = dir + "/newMap.json";
            if (File.Exists(path))
            {
                var file = File.ReadAllText(path);
                _mapGenCode.GetMapFile(file);
            }
        }

        if (randomMap)
        {
            _mapGenCode.InitMapGenerator("random", _tileMaker);

            int xSize = 18;
            int ySize = 18;
            //Get size
            switch(size)
            {
                case MapSize.small:
                    xSize = 32;
                    ySize = 32;
                    break;
                case MapSize.medium:
                    xSize = 64;
                    ySize = 64;
                    break;
                case MapSize.big:
                    xSize = 128;
                    ySize = 128;
                    break;
                case MapSize.mega:
                    xSize = GlobalValues.MaxMapSize;
                    ySize = GlobalValues.MaxMapSize;
                    break;
            }


            _mapGenCode.RandomlyGenerateMap(randTreshold, generatorSeed, smoothing, xSize, ySize);
        }
        else
        {
            if (!canFindMap)
            {
                Debug.Log("Map jason file NULL, use default");
                _mapGenCode.InitMapGenerator("null", _tileMaker);
                _mapGenCode.GenerateMap();
            }
            else
            {
                _mapGenCode.InitMapGenerator(_mapJsonName, _tileMaker);
                _mapGenCode.GenerateMap();
            }

        }


        switch(_mapGenCode.GetMapTheme())
        {
            case 1:
                _musicSource.clip = _defaultMapSong[0];
                _musicSource.Play();
                break;
            case 2:
                _musicSource.clip = _defaultMapSong[0];
                _musicSource.Play();
                break;
            case 3:
                _musicSource.clip = _defaultMapSong[0];
                _musicSource.Play();
                break;
            case 4:
                _musicSource.clip = _defaultMapSong[0];
                _musicSource.Play();
                break;
            case 5:
                _musicSource.clip = _defaultMapSong[0];
                _musicSource.Play();
                break;
            default:
                _musicSource.clip = _defaultMapSong[0];
                _musicSource.Play();
                break;

        }
    }

    private void OnEnable()
    {
        OnGetMaterial += GetMaterialList;
    }

    private void OnDisable()
    {
        OnGetMaterial -= GetMaterialList;
    }

    void OnGUI()
    {
        /*
        if (_mapGenCode.mapFinished)
            for (int x = 0; x < _mapGenCode.mapGrid.GetLength(0); x++)
                for (int y = 0; y < _mapGenCode.mapGrid.GetLength(1); y++)
                {
                    Texture2D newT = new Texture2D(1, 1);
                    newT.SetPixel(1, 1, Color.red);

                    if (_mapGenCode.mapGrid[x, y] == 0)
                        newT.SetPixel(1, 1, Color.red);
                    else
                         if (_mapGenCode.mapGrid[x, y] == 1)
                        newT.SetPixel(1, 1, Color.green);

                    GUI.DrawTexture(new Rect(10, 10, 10 * x, 10 * y), newT, ScaleMode.ScaleToFit, false, 10.0f);

                }
        */
    }
}
