using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Region
{
    Sand,
    Soil,
    Grass,
    Snow
}

[System.Serializable]
public struct BlockInfo
{
    public Region region;
    public bool isVisible;
    public GameObject block;
    public BlockInfo(Region type, bool isVisible, GameObject block)
    {
        this.region = type;
        this.isVisible = isVisible;
        this.block = block;
    }
}
[System.Serializable]
public struct BlockPrefabInfo
{
    public Region region;
    public int height;
    public GameObject block;
}

[System.Serializable]
public struct MapObjectPrefabInfo
{
    public Region region;
    public GameObject mapObject;
    public MapObjectPrefabInfo(Region type, GameObject mapObject)
    {
        this.region = type;
        this.mapObject = mapObject;
    }
}

public class BlockMapGenerator : MonoBehaviour
{
    static public int widthX = 225;
    static public int widthZ = 225;
    static public int height = 225;

    [Header("���")]
    public BlockPrefabInfo[] blockPrefabInfos;

    //public GameObject prefab_BlackBlock;
    //public GameObject prefab_BrownBlock;

    //public GameObject prefab_SnowBlock;
    //public GameObject prefab_GroundBlock;
    //public GameObject prefab_GrassBlock;

    //public GameObject prefab_CoalBlock;
    //public GameObject prefab_MetalBlock;
    //public GameObject prefab_GoldBlock;
    //public GameObject prefab_FloorBlock; // �ٴں�(�ı��Ұ�)


    [Header("ȯ�� ������Ʈ")]
    public MapObjectPrefabInfo[] envirionmentsInfos;
    public int prob_NonObject; // ������Ʈ�� ������ ���� Ȯ�� (0~100)

    [Header("������")]
    public float waveLength = 0;
    public float amplitude = 0;

    public BlockInfo[,,] worldBlocks = new BlockInfo[widthX, height, widthZ];

    public float groundHeightOffset = 20;
    public bool isFinishGeneration = false;
    public float progress = 0;
    private int seed;

    // Start is called before the first frame update
    void Start()
    { 
        StartCoroutine(InitGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator InitGame()
    {
        // �� ����

        yield return StartCoroutine(MapInit());

        // ����1

        // ����2
    }

    IEnumerator MapInit()
    {
        seed = Random.Range(0, 100); // �ӽ�

        for (int x = 0; x < widthX; x++)
        {
            progress = x / (float)widthX * 100;
            for (int z = 0; z < widthZ; z++)
            {
                float xCoord = (x + 0) / waveLength;
                float zCoord = (z + 0) / waveLength;
                int noiseValueY = (int)(Mathf.PerlinNoise(xCoord, zCoord) * amplitude + groundHeightOffset);

                Vector3 pos = new Vector3(x, noiseValueY, z);
                StartCoroutine(CreateBlock(noiseValueY, pos, true));

                // ������ Y���� �� ��ġ �� �� �غκк��ʹ� ����� �������� �ʰ� ������ �迭�� ������
                for (int y = noiseValueY - 1; y > 0; y--)
                {
                    pos = new Vector3(x, y, z);
                    StartCoroutine(CreateBlock(y, pos, false));
                }   
            }
            yield return null;
        }
        Debug.Log("������");
        progress = 100;
        isFinishGeneration = true;
    }
    IEnumerator CreateBlock(int y, Vector3 blockPos, bool visible)
    {
        for(int i = 0; i<blockPrefabInfos.Length; i++)
        {
            if(blockPrefabInfos[i].height < y)
            {
                if (visible)
                {
                    GameObject block = Instantiate(blockPrefabInfos[i].block, blockPos, Quaternion.identity);

                    worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(blockPrefabInfos[i].region, visible, block);

                    // ������ ��� ���� ������Ʈ ��ġ
                    StartCoroutine(CreateObject(blockPos + Vector3.up, blockPrefabInfos[i].region));
                }
                else
                {
                    worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(blockPrefabInfos[i].region, visible, null);
                }
                break;
            }
        }

        /*if(y>40)
        {
            if(visible)
            {
                GameObject block = Instantiate(prefab_SnowBlock, blockPos, Quaternion.identity);
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(Region.Snow, visible, block);
            }
            else
            {
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(Region.Snow, visible, null);
            }
        }

        else if (y > 35)
        {
            if (visible)
            {
                GameObject block = Instantiate(prefab_GrassBlock, blockPos, Quaternion.identity);
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(Region.Grass, visible, block);
            }
            else
            {
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(Region.Grass, visible, null);
            }
        }
        else if (y > 20)
        {
            if (visible)
            {
                GameObject block = Instantiate(prefab_GroundBlock, blockPos, Quaternion.identity);
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(Region.Grass, visible, block);
            }
            else
            {
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(Region.Grass, visible, null);
            }
        }

        int prob = 3;
        // ���� 0~7 �������� prob Ȯ���� ���ҽ� ��� ����
        if (y > 0 && y < 7 && Random.Range(0, 100) < prob)
        {
            if (worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].block != null)
            {
                Destroy(worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].block);
            }

            if (visible)
            {
                GameObject block = Instantiate(prefab_CoalBlock, blockPos, Quaternion.identity);
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(Region.Soil, true, block);
            }
            else
            {
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(Region.Soil, true, null);
            }
            
        }

        if(y==0)
        {
            if(visible)
            {
                if (visible)
                {
                    GameObject block = Instantiate(prefab_FloorBlock, blockPos, Quaternion.identity);
                    worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(Region.Soil, visible, block);
                }
                else
                {
                    worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(Region.Soil, visible, null);
                }
            }
        }*/

        yield return null;
    }
    IEnumerator CreateObject(Vector3 objectPos, Region region)
    {
        if (Random.Range(0, 100) >= prob_NonObject)
        {
            // �� ������ �´� ������Ʈ ����Ʈ�� �������� ������ ������Ʈ ���� �ڷ�ƾ ����
            if(System.Array.FindIndex(envirionmentsInfos, info => info.region == region) < 0)
            {
                yield break;
            }

            // ������Ʈ Ÿ���� ���� ������ ��ġ�Ҷ����� �����ϰ� ã��
            int objectIndex = 0;
            while (true)
            {
                objectIndex = Random.Range(0, envirionmentsInfos.Length);
                if (envirionmentsInfos[objectIndex].region == region)
                {
                    break;
                }
            }

            GameObject environment = Instantiate(envirionmentsInfos[objectIndex].mapObject, objectPos, Quaternion.identity);
        }
        yield return null;
    }


}
