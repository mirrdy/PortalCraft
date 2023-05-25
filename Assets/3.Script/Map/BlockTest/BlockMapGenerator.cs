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

    [Header("블록")]
    public BlockPrefabInfo[] blockPrefabInfos;

    //public GameObject prefab_BlackBlock;
    //public GameObject prefab_BrownBlock;

    //public GameObject prefab_SnowBlock;
    //public GameObject prefab_GroundBlock;
    //public GameObject prefab_GrassBlock;

    //public GameObject prefab_CoalBlock;
    //public GameObject prefab_MetalBlock;
    //public GameObject prefab_GoldBlock;
    //public GameObject prefab_FloorBlock; // 바닥블럭(파괴불가)


    [Header("환경 오브젝트")]
    public MapObjectPrefabInfo[] envirionmentsInfos;
    public int prob_NonObject; // 오브젝트가 생기지 않을 확률 (0~100)

    [Header("맵정보")]
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
        // 맵 생성

        yield return StartCoroutine(MapInit());

        // 생성1

        // 생성2
    }

    IEnumerator MapInit()
    {
        seed = Random.Range(0, 100); // 임시

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

                // 노이즈 Y값에 블럭 설치 후 그 밑부분부터는 블록을 생성하지 않고 정보만 배열에 저장함
                for (int y = noiseValueY - 1; y > 0; y--)
                {
                    pos = new Vector3(x, y, z);
                    StartCoroutine(CreateBlock(y, pos, false));
                }   
            }
            yield return null;
        }
        Debug.Log("생성끝");
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

                    // 생성한 블록 위에 오브젝트 설치
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
        // 높이 0~7 범위에서 prob 확률로 리소스 블록 생성
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
            // 맵 영역에 맞는 오브젝트 리스트를 갖고있지 않으면 오브젝트 생성 코루틴 종료
            if(System.Array.FindIndex(envirionmentsInfos, info => info.region == region) < 0)
            {
                yield break;
            }

            // 오브젝트 타입이 현재 지역과 일치할때까지 랜덤하게 찾음
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
