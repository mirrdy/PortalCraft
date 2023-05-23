using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMapGeneratorForNoise : MonoBehaviour
{
    [System.Serializable]
    public struct BlockType
    {
        public string name;
        public float height;
        public GameObject block;
    }
    [Header("블록 프리팹")]
    public GameObject block_Black;
    public GameObject block_Brown;

    [Header("블록 생성 정보")]
    public BlockType[] blocks;


    [Header("맵정보")]
    public int widthX = 0;
    public int widthZ = 0;
    public float waveLength = 0;
    public float amplitude = 0;

    public float noiseScale;

    public int octaves;
    private int seed;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public bool isTest = false;

    private List<GameObject> list_Block = new List<GameObject>();

    private void Start()
    {
        //for (int x = 0; x < widthX; x++)
        //{
        //    for (int z = 0; z < widthZ; z++)
        //    {
        //        list_Block.Add(Instantiate(block_Black, new Vector3(x, 0, z), Quaternion.identity));
        //    }
        //}


        //for (int i = 0; i < list_Block.Count; i++)
        //{
        //    float xCoord = list_Block[i].transform.position.x / waveLength;
        //    float zCoord = list_Block[i].transform.position.z / waveLength;
        //    int y = (int)(Mathf.PerlinNoise(xCoord, zCoord) * amplitude);
        //    list_Block[i].transform.position = new Vector3(list_Block[i].transform.position.x, y, list_Block[i].transform.position.z);
        //}
        seed = Random.Range(-10000, 10000);

        Vector3 offset = new Vector3(0, 0);

        float[,] noiseMap = Noise.GenerateNoiseMap(widthX, widthZ, seed, noiseScale, octaves, persistance, lacunarity, offset);

        for (int z = 0; z < widthZ; z++)
        {
            for (int x = 0; x < widthX; x++)
            {
                float currentHeight = noiseMap[x, z];
                for (int i = 0; i < blocks.Length; i++)
                {
                    if (currentHeight <= blocks[i].height)
                    {
                        list_Block.Add(Instantiate(blocks[i].block, new Vector3(x, (int)currentHeight * amplitude, z), Quaternion.identity));
                        break;
                    }
                }
            }
        }
    }
    public void GenerateMap()
    {
        Vector3 offset = new Vector3(0, 0);

        float[,] noiseMap = Noise.GenerateNoiseMap(widthX, widthZ, seed, noiseScale, octaves, persistance, lacunarity, offset);

        for (int z = 0; z < widthZ; z++)
        {
            for (int x = 0; x < widthX; x++)
            {
                float currentHeight = noiseMap[x, z];
                for (int i = 0; i < blocks.Length; i++)
                {
                    if (currentHeight <= blocks[i].height)
                    {
                        //list_Block[z * widthX + x].GetComponentInChildren<MeshRenderer>().sharedMaterial = blocks[i].block.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                        MeshRenderer[] tmpRenderers = list_Block[z * widthX + x].GetComponentsInChildren<MeshRenderer>();

                        list_Block[z * widthX + x].transform.position = new Vector3(x, (int)(currentHeight * amplitude), z);
                        break;
                    }
                }
            }
        }
    }
    private void Update()
    {
        /* for (int i = 0; i < list_Block.Count; i++)
         {
             float xCoord = list_Block[i].transform.position.x / waveLength;
             float zCoord = list_Block[i].transform.position.z / waveLength;
             int y = (int)(Mathf.PerlinNoise(xCoord, zCoord) * amplitude);
             list_Block[i].transform.position = new Vector3(list_Block[i].transform.position.x, y, list_Block[i].transform.position.z);
         }*/
    }
}
