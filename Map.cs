using System.Collections;
using UnityEngine;

public enum MapTheme
{
    mapTheme_Castle,
    mapTheme_Dungeon,
    mapTheme_Cave,
    mapTheme_Grotto,
    mapTheme_TheVoid
};

public enum MapSize
{
    small,
    medium,
    big,
    mega,
};


[System.Serializable]
public class Map
{

    private int _mapTheme = 0;

    public int _wallTexture = 0;
    public int _floorTexture = 0;

    public bool smoothingBool = true;

    //For the JSON, move 2d array to these
    public int[] mapGridX = new int[GlobalValues.MaxMapSize];
    public int[] mapGridY = new int[GlobalValues.MaxMapSize];

    private Material _ceilingTexture = null;

    private Material _wallMat = null;
    private Material _floorMat = null;

    private Material[] _materialList = null;

    private TileMaker _tileMaker = null;

    public bool mapFinished = false;

    //1 = room
    //3 = breakablewall
    //3 = enemy
    //0 = empty space

    private int[,] defaultMapGrid = {
         { 1, 1, 1, 0, 1 ,1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
         { 0, 0, 1, 0, 1 ,1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
         { 1, 0, 1, 1, 1 ,1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
         { 1, 1, 1, 1, 1 ,1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
         { 1, 0, 0, 0, 1 ,1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
         { 1, 1, 1, 1, 1 ,1, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
         { 1, 1, 1, 1, 1 ,1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
         { 0, 1, 0, 1, 1 ,1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
         { 1, 1, 0, 1, 1 ,1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
         { 0, 0, 0, 0, 0 ,0, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
         { 1, 1, 1, 1, 1 ,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1},
         { 1, 1, 1, 1, 1 ,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
         { 1, 1, 1, 1, 1 ,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
         { 1, 1, 1, 1, 1 ,1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
         { 1, 1, 1, 1, 1 ,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1},
         { 1, 1, 1, 1, 1 ,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1},
         { 1, 1, 1, 1, 1 ,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 0, 1, 1, 1, 1, 1},
         { 1, 1, 1, 0, 1 ,1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1},
         { 1, 1, 1, 1, 1 ,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1},
         { 1, 1, 1, 1, 1 ,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1},
         { 1, 1, 1, 1, 1 ,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1},
         { 1, 1, 1, 1, 1 ,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1},
         { 1, 1, 1, 1, 1 ,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1},
         { 1, 1, 1, 1, 1 ,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1}
    };
    
    public int[,] mapGrid = new int[GlobalValues.MaxMapSize, GlobalValues.MaxMapSize];

    public int GetMapTheme()
    {
        return _mapTheme;
    }

    public void SetMapGrid(int[,] map) {mapGrid = map; }

    public Map GetMapFile(string jsonString)
    {
        return JsonUtility.FromJson<Map>(jsonString);
    }

    public string MakeMapFile()
    {
        int maxSize = GlobalValues.MaxMapSize;
        //move 2d array to 1d
        System.Buffer.BlockCopy(mapGrid, maxSize, mapGridX, 0, maxSize);
        System.Buffer.BlockCopy(mapGrid, maxSize, mapGridY, 0, maxSize);
        return JsonUtility.ToJson(this);
    }

    public void InitMapGenerator(string json, TileMaker tM)
    {
        //set up map array and textures
        _materialList = MapSceneSetUp.OnGetMaterial();

        //Set the map grid to the defaut if its null
        if (json == "null")
        {
            mapGrid = defaultMapGrid;
        }

        //Get list from TileMaker
        GetMaterials(_wallTexture, _floorTexture);
        _tileMaker = tM;
        _tileMaker.SetTextures(_wallMat, _floorMat);
    }

    private bool CheckRoomBounds(int x1, int y1, int x2, int y2)
    {



        return false;
    }

    public void RandomlyGenerateMap(int randTresh, string seed = "", int smoothing = 2, int mWidth = 18, int mHeight = 18)
    {
        if (seed == "")
            seed = Time.realtimeSinceStartup.ToString();

        /*

        System.Random puse = new System.Random(seed.GetHashCode());

        //empty the default map
        mapGrid = new int[mWidth, mHeight];

        int numberofRooms = puse.Next(5, 25);
        Vector2[] rooms = new Vector2[numberofRooms];

        int distFromStart = 0;

        //start the first room
        int xStart = puse.Next(2, 8);
        int yStart = puse.Next(2, 8);
        for (int row = 0; row < xStart; row++)
            for (int index = 0; index < yStart; index++)
            {
                mapGrid[row, index] = 1;
                distFromStart++;
            }

        //now go through each room and check that we
        //can place them
        Debug.Log(numberofRooms);
        for (int nr = 0; nr < numberofRooms; nr++)
        {
            bool valid = false;
            while(!valid)
            {
                int xDir = 0;
                int yDir = 0;
                //pick a direction to move in
                //1 = down, 2 = right
                int dir = 0;
                if (distFromStart > 16)
                    dir = puse.Next(1, 4);
                else
                    dir = puse.Next(1, 2);

                Debug.Log(dir);

                if (dir == 1)
                    yDir = 1; //down
                else if (dir == 2)
                    xDir = 1; //right
                else if (dir == 3)
                    yDir = -1; //up
                else if(dir == 4)
                    xDir = -1;//left

                //get new size
                int xS = puse.Next(3, 8);
                int yS = puse.Next(3, 8);
                int connectorLength = puse.Next(3, 8);

                //create the pivot point for the new room
                int xP = xStart + (xDir * connectorLength);
                int yP = yStart + (yDir * connectorLength);

                //make new room
                for (int x = xP; x < xP + xS; x++)
                    for (int y = yP; y < yP + yS; y++)
                    {
                        if (x > 0 && y > 0)
                            if (x < mWidth && y < mHeight)
                                mapGrid[x, y] = 1;
                    }

                //now add the connector
                if(xDir == 1)
                {
                    //right
                    for (int cx = xStart - 1; cx < xStart + connectorLength; cx++)
                        if (cx > 0 && cx < mWidth)
                            if (yStart > 0 && yStart < mHeight)
                                    mapGrid[cx, yStart] = 2;
                }
                else if (xDir == -1)
                {
                    //left
                    for (int cx = xStart + 1; cx > xStart - connectorLength; cx--)
                        if (cx > 0 && cx < mWidth)
                            if (yStart > 0 && yStart < mHeight)
                                mapGrid[cx, yStart] = 1;
                }
                else if (yDir == 1)
                {
                    //down
                    for (int cy = yStart - 1; cy < yStart + connectorLength; cy++)
                        if (cy > 0 && cy < mHeight)
                            if (xStart > 0 && xStart < mWidth)
                                    mapGrid[xStart, cy] = 2;
                }
                else if (yDir == -1)
                {
                    //up
                    for (int cy = yStart + 1; cy > yStart - connectorLength; cy--)
                        if (cy > 0 && cy < mHeight)
                            if (xStart > 0 && xStart < mWidth)
                                mapGrid[xStart, cy] = 1;
                }

                xStart = xP + xS;
                yStart = yP + yS;

                //add 4 just to give it some leeway
                distFromStart += 4;
                valid = true;
            }
        }


        //now make the map
        for (int row = 0; row < mapGrid.GetLength(0); row++)
        {
            Vector3 position = Vector3.zero;

            for (int index = 0; index < mapGrid.GetLength(1); index++)
            {
                position = Vector3.zero + Vector3.right * (index * 3) + Vector3.forward * (row * 3);
                PickTileAndSpawn(mapGrid[row, index], position, row, index);
            }
        }

        Debug.Log(distFromStart);
        mapFinished = true;
        */
   
        
        

    }

    public void GenerateMap()
    {
        for (int row = 0; row < mapGrid.GetLength(0); row++)
        {
            Vector3 position = Vector3.zero;


            for (int index = 0; index < mapGrid.GetLength(1); index++)
            {
                position = Vector3.zero + Vector3.right * (index * 3) + Vector3.forward * (row * 3);
                PickTileAndSpawn(mapGrid[row, index],position, row, index);
            }
        }
    }

    private void GetMaterials(int wallIndex, int floorIndex)
    {
        if (wallIndex < _materialList.Length || wallIndex >= 0)
            _wallMat = _materialList[wallIndex];

        if (floorIndex < _materialList.Length || floorIndex >= 0)
            _floorMat = _materialList[floorIndex];

    }

    private void PickTileAndSpawn(int tileType,Vector3 pos, int row, int index)
    {
        //empty space, go back
        if (tileType == 0)
            return;


        //pick/make the tile
        if (tileType == 1)
        {
            _tileMaker.MakeFloor(pos);

            //if tile behinde in the index is empty, make wall
            if (index == 0 || mapGrid[row, index - 1] == 0 && index > 0)
            {
                _tileMaker.MakeWall(pos, -Vector3.right);
            }
            
            if (index == mapGrid.GetLength(1) - 1 || mapGrid[row, index + 1] == 0)
            {
                _tileMaker.MakeWall(pos, Vector3.right);
            }
            
            if (row == 0 || mapGrid[row - 1, index] == 0)
            {
                //pos.z -= 3;
                _tileMaker.MakeWall(pos, -Vector3.up);
            }
           
            if (row == mapGrid.GetLength(0) - 1 || mapGrid[row + 1, index] == 0)
                _tileMaker.MakeWall(pos, Vector3.up);
        }
        else if (tileType == 2)
        {
            _tileMaker.MakeFloor(pos);

            //only make a breakable wall if theres a tile ahead
            //else make a standard static wall
            if (index == 0 || mapGrid[row, index - 1] == 0 && index > 0)
                _tileMaker.MakeWall(pos, -Vector3.right);
            else if (index == 1 || mapGrid[row, index - 1] == 1 && index > 0)
                _tileMaker.MakeWall(pos, -Vector3.right, true);

            if (index == mapGrid.GetLength(1) - 1 || mapGrid[row, index + 1] == 0)
                _tileMaker.MakeWall(pos, Vector3.right);
            else if (index == mapGrid.GetLength(1) - 1 || mapGrid[row, index + 1] == 1)
                _tileMaker.MakeWall(pos, Vector3.right, true);

            if (row == 0 || mapGrid[row - 1, index] == 0)
                _tileMaker.MakeWall(pos, -Vector3.up);
            else if (row == 1 || mapGrid[row - 1, index] == 1)
            {
                _tileMaker.MakeWall(pos, -Vector3.up, true);
                //invert this one
                _tileMaker.MakeWall(pos, Vector3.up, true);
            }

            if (row == mapGrid.GetLength(0) - 1 || mapGrid[row + 1, index] == 0)
                _tileMaker.MakeWall(pos, Vector3.up);
            else if (row == mapGrid.GetLength(0) - 1 || mapGrid[row + 1, index] == 0)
                _tileMaker.MakeWall(pos, Vector3.up, true);
        }
        else if (tileType == 3)
        {
            _tileMaker.MakeFloor(pos, 1);

            //only make a breakable wall if theres a tile ahead
            //else make a standard static wall
            if (index == 0 || mapGrid[row, index - 1] == 0 && index > 0)
                _tileMaker.MakeWall(pos, -Vector3.right);
            else if (index == 1 || mapGrid[row, index - 1] == 1 && index > 0)
                _tileMaker.MakeWall(pos, -Vector3.right, true);

            if (index == mapGrid.GetLength(1) - 1 || mapGrid[row, index + 1] == 0)
                _tileMaker.MakeWall(pos, Vector3.right);
            else if (index == mapGrid.GetLength(1) - 1 || mapGrid[row, index + 1] == 1)
                _tileMaker.MakeWall(pos, Vector3.right, true);

            if (row == 0 || mapGrid[row - 1, index] == 0)
                _tileMaker.MakeWall(pos, -Vector3.up);
            else if (row == 1 || mapGrid[row - 1, index] == 1)
            {
                _tileMaker.MakeWall(pos, -Vector3.up, true);
                //invert this one
                _tileMaker.MakeWall(pos, Vector3.up, true);
            }

            if (row == mapGrid.GetLength(0) - 1 || mapGrid[row + 1, index] == 0)
                _tileMaker.MakeWall(pos, Vector3.up);
            else if (row == mapGrid.GetLength(0) - 1 || mapGrid[row + 1, index] == 0)
                _tileMaker.MakeWall(pos, Vector3.up, true);
        }


        //spawn it
        //GameObject testTile = Instantiate(tileToSpawn, pos, transform.rotation);
    }

}
