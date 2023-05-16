using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        float sampleX;
        float sampleY;

        if(scale <= 0)
        {
            scale = 0.0001f;
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                sampleX = x / scale;
                sampleY = y / scale;

                float perlineValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlineValue;
            }
        }
        return noiseMap;
    }
}
