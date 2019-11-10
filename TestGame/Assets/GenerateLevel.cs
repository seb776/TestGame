using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : Singleton<GenerateLevel>
{
    public Material BackMat;
    public Material FrontMat;
    private void _fromIntArray(int startX, int startZ, int[,] map, int deco)
    {
        float cubeThick = 0.5f;
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
                    cube.transform.localScale = new Vector3(1.0f, cubeThick, 1.0f);
                    cube.transform.position += new Vector3((float)j - (float)map.GetLength(1) * 0.5f, cubeThick * 0.5f, -(i - map.GetLength(0) * 0.5f));
                    cube.GetComponent<MeshRenderer>().material = FrontMat;
                }
            }
        }
    }

    public void FromFile(string file)
    {
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
        _fromIntArray(0, 0, arr, 0);
    }
}
