using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenerateLevel : Singleton<GenerateLevel>
{
    public Material BackMat;
    public Material FrontMat;
    private void _fromIntArray(int[,] map, int deco)
    {
        float cubeThick = 2.0f;
        var goBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
        goBack.GetComponent<MeshRenderer>().material = BackMat;

        goBack.transform.localScale = new Vector3(40.0f, cubeThick, 40.0f);

        for (int i = 0; i < map.GetLength(0); ++i)
        {
            for (int j = 0; j < map.GetLength(1); ++j)
            {
                if (map[i, j] == 1)
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.tag = "mapPart";
                    cube.transform.localScale = new Vector3(1.0f, cubeThick, 1.0f);
                    cube.transform.position += new Vector3((float)j, cubeThick * 0.5f, map.GetLength(0) - (i + 1));
                    cube.GetComponent<MeshRenderer>().material = FrontMat;
                }
            }
        }
        var goTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var goDown = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var goLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var goRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        goTop.tag = "mapPart";
        goDown.tag = "mapPart";
        goLeft.tag = "mapPart";
        goRight.tag = "mapPart";

        goTop.transform.localScale = new Vector3(40.0f, cubeThick, 20.0f);
        goTop.transform.position += new Vector3(0.0f, cubeThick * 0.5f, ((map.GetLength(0))) + 10.0f + 0.5f - 1.0f);
        goTop.GetComponent<MeshRenderer>().material = FrontMat;

        goDown.transform.localScale = new Vector3(40.0f, cubeThick, 20.0f);
        goDown.transform.position += new Vector3(0.0f, cubeThick * 0.5f, -10.0f + 0.5f - 1.0f);
        goDown.GetComponent<MeshRenderer>().material = FrontMat;

        goLeft.transform.localScale = new Vector3(20.0f, cubeThick, (map.GetLength(0)));
        goLeft.transform.position += new Vector3(-0.5f - 10.0f, cubeThick * 0.5f, 0.5f * map.GetLength(0) + 0.5f - 1.0f);
        goLeft.GetComponent<MeshRenderer>().material = FrontMat;
        //Mathf.Round
        goRight.transform.localScale = new Vector3(20.0f, cubeThick, (map.GetLength(0)));
        goRight.transform.position += new Vector3(((float)map.GetLength(1) - 1.0f) + 10.0f + 0.5f, cubeThick * 0.5f, 0.5f * map.GetLength(0) + 0.5f - 1.0f);
        goRight.GetComponent<MeshRenderer>().material = FrontMat;

        Camera.main.transform.position = new Vector3(((float)map.GetLength(1) - 1.0f) * 0.5f, Camera.main.transform.position.y, 0.5f * map.GetLength(0) - 2.0f);
    }

    public static void ClearMapParts()
    {
        foreach (var go in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (go.tag == "mapPart")
                Destroy(go);
        }
    }

    public static void GenerateRandomMap(string path, int height, int width)
    {
        UnityEngine.Random.InitState((int)DateTime.UtcNow.ToFileTimeUtc());

        int[,] arr = new int[height, width];
        for (int i = 0; i < arr.GetLength(0); ++i)
        {

            for (int j = 0; j < arr.GetLength(1); ++j)
            {

                arr[i, j] = 1;
            }
        }
        Vector2Int startPos = new Vector2Int(UnityEngine.Random.Range(0, width), height - 1);

        int traceCount = UnityEngine.Random.Range(10, 25);
        for (int i = 0; i < traceCount; ++i)
        {
            int dirInt = UnityEngine.Random.Range(1, 5);
            Vector2Int dir = Vector2Int.zero;
            if (dirInt == 1)
                dir = Vector2Int.up;
            if (dirInt == 2)
                dir = Vector2Int.down;
            if (dirInt == 3)
                dir = Vector2Int.left;
            if (dirInt == 4)
                dir = Vector2Int.right;
            int caseCount = UnityEngine.Random.Range(1, 10);
            for (int j = 0; j < caseCount; ++j)
            {
                arr[startPos.y, startPos.x] = 0;
                if ((startPos.x + dir.x) < 0 || (startPos.x + dir.x) >= width || (startPos.y + dir.y) < 0 || (startPos.y + dir.y) >= height)
                    break;
                else
                    startPos += dir;
            }
        }

        StringBuilder sb = new StringBuilder();
        sb.Append("int[,] arr = new int[,]\n{\n");
        for (int i = 0; i < height; ++i)
        {
            sb.Append("{");
            for (int j = 0; j < width; ++j)
            {
                sb.Append(arr[i, j]);
                if (j != (width - 1))
                    sb.Append(",");
            }
            sb.Append("},\n");
        }
        sb.Append("};\n");
        File.WriteAllText(path, sb.ToString());
    }

    public Map LoadLevel(int lvl)
    {
        var maps = new Func<Map>[]
        {
            GetMapA,
            GetMapB,
            GetMapC,
            GetMapD,
            GetMapE,
            GetMapF,
        };

        Map a = maps[lvl%maps.Length]();

        _fromIntArray(a.GameMap, 0);
        a.ToFillCount = 0;
        foreach (var v in a.GameMap)
        {
            if (v == 0)
                a.ToFillCount++;
        }
        return a;
    }

    public class Map
    {
        public int[,] GameMap;
        public Vector2Int Start;
        public int ToFillCount;
        public int Access(Vector2Int i)
        {
            return GameMap[Mathf.Min(Mathf.Max(GameMap.GetLength(0) - 1 - i.y, 0), GameMap.GetLength(0) - 1), Mathf.Min(Mathf.Max(i.x, 0), GameMap.GetLength(1))];
        }
        public void Set(Vector2Int i, int val)
        {
            GameMap[Mathf.Min(Mathf.Max(GameMap.GetLength(0) - 1 - i.y, 0), GameMap.GetLength(0) - 1), Mathf.Min(Mathf.Max(i.x, 0), GameMap.GetLength(1))] = val;
        }
    }

    public static Vector2Int FloatToIntCoordinates(Vector2 vec)
    {
        return new Vector2Int((int)Mathf.Round(vec.x), (int)Mathf.Round(vec.y));
    }

    public static Vector2 IntToFloatCoordinates(Vector2Int vec)
    {
        return vec;
    }


    public Map GetMapA()
    {
        Map map = new Map();
        int[,] arr = new int[,]
        {
            {0,0,0,0,1,1,1},
            {0,1,1,0,1,1,1},
            {0,0,0,0,0,0,0},
            {1,1,1,0,1,1,0},
            {1,1,1,0,1,1,0},
            {1,1,1,0,1,1,0},
            {1,1,1,0,1,1,0},
            {1,1,1,0,1,1,0},
            {1,1,1,0,1,1,0},
            {1,1,1,0,1,1,0},
            {1,1,1,0,1,1,0},
            {1,1,1,0,1,1,0},
            {0,1,1,0,1,1,0},
            {0,0,0,0,0,0,0},
        };
        map.GameMap = arr;
        map.Start = new Vector2Int(0, 1);
        return map;
    }
    public Map GetMapB()
    {
        Map map = new Map();
        int[,] arr = new int[,]
        {
        {1,1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1,1},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,1,1,0},
        {1,0,1,1,1,1,1,1,1,0},
        {1,0,1,1,1,1,1,1,1,0},
        {1,0,1,1,1,1,1,1,1,0},
        {1,0,1,1,1,1,1,1,1,0},
        {1,0,0,0,0,0,0,0,0,0},
        };
        map.GameMap = arr;
        map.Start = new Vector2Int(1, 0);
        return map;
    }
    public Map GetMapC()
    {
        Map map = new Map();
        int[,] arr = new int[,]
{
        {0,0,0,0,0,1,1,1,1,1},
        {0,0,0,0,0,0,1,1,1,1},
        {0,1,1,1,1,0,1,1,1,1},
        {0,1,1,1,1,0,1,1,1,1},
        {0,1,1,1,1,0,1,1,1,1},
        {0,1,1,1,1,0,1,1,1,1},
        {1,1,1,1,1,0,0,0,0,0},
        {1,1,1,1,1,1,1,1,1,0},
        {1,1,1,1,1,1,1,1,1,0},
        {0,0,0,0,0,0,0,0,0,0},
};
        map.GameMap = arr;
        map.Start = new Vector2Int(0, 0);
        return map;
    }

    public Map GetMapD()
    {
        Map map = new Map();
        int[,] arr = new int[,]
{
        {1,1,1,1,0,0,0,0,0,0},
        {1,1,1,1,1,1,1,1,1,0},
        {1,1,1,1,1,1,1,1,1,0},
        {1,1,1,1,1,1,1,1,1,0},
        {1,1,1,1,1,0,0,0,0,0},
        {1,1,1,1,1,0,1,1,1,0},
        {1,1,1,1,1,0,1,1,1,0},
        {1,1,1,1,1,0,0,0,0,0},
        {1,1,1,1,1,0,1,1,1,1},
        {1,1,1,1,1,0,1,1,1,1},
};
        map.GameMap = arr;
        map.Start = new Vector2Int(5, 0);
        return map;
    }

    public Map GetMapE()
    {
        Map map = new Map();
        int[,] arr = new int[,]
        {
        {0,0,0,0,0,1,1},
        {1,1,1,1,0,1,1},
        {1,1,1,1,0,1,1},
        {1,1,1,1,0,1,1},
        {1,1,1,1,0,1,1},
        {1,1,1,1,0,1,1},
        {1,1,1,1,0,1,1},
        {1,1,1,1,0,0,0},
        {1,1,1,1,1,1,0},
        {1,1,1,1,1,1,0},
        {1,1,1,1,1,1,0},
        {1,1,1,1,1,1,0},
        };
        map.GameMap = arr;
        map.Start = new Vector2Int(6, 0);
        return map;
    }
    public Map GetMapF()
    {
        Map map = new Map();
        int[,] arr = new int[,]
{
                    { 1,0,0,0,0,1,1 },
                    { 0,0,0,0,0,1,1 },
                    { 0,0,1,0,0,0,1 },
                    { 0,0,0,0,0,0,0 },
                    { 1,0,1,0,1,0,0 },
                    { 1,0,1,0,1,0,0 },
                    { 0,0,1,0,1,0,0 },
                    { 0,1,1,0,1,0,0 },
                    { 0,1,1,0,1,0,0 },
                    { 0,0,1,0,1,0,0 },
                    { 1,0,1,0,1,0,0 },
                    { 1,0,1,0,1,0,0 },
};
        map.GameMap = arr;
        map.Start = new Vector2Int(1, 0);
        return map;
    }
}
