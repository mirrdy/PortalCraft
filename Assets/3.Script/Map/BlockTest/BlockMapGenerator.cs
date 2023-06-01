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
    public bool isExist;
    public BlockInfo(Region type, bool isVisible, GameObject block, bool isExist)
    {
        this.region = type;
        this.isVisible = isVisible;
        this.block = block;
        this.isExist = isExist;
    }
}
[System.Serializable]
public struct BlockPrefabInfo
{
    public Region region;
    public int height;
    public bool isFlat;
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
    public static BlockMapGenerator instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

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
    public GameObject monsterSpawnerInfo;
    private Vector3 monsterSpawnerPos;
    private bool isCreatedSpawner;
    private bool isCreatedPortal;

    [Header("맵정보")]
    public float waveLength = 0;
    public float amplitude = 0;

    private int seed;

    public BlockInfo[,,] worldBlocks = new BlockInfo[widthX, height, widthZ];

    public float groundHeightOffset = 20;
    public bool isFinishGeneration = false;
    public float progress = 0;

    [Header("포탈")]
    public GameObject portalInfo;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitGame());
    }

    IEnumerator InitGame()
    {
        PlayerControl.instance.enabled = false;
        // 맵 생성
        yield return StartCoroutine(MapInit());

        // 포탈 생성
        while (true)
        {
            if (isFinishGeneration)
            {
                break;
            }
            yield return null;
        }

        Instantiate(monsterSpawnerInfo, monsterSpawnerPos, Quaternion.identity);
        PlayerControl.instance.enabled = true;

        // 생성2
    }

    IEnumerator MapInit()
    {
        float randomOffsetX = Random.Range(0, 100);
        float randomOffsetZ = Random.Range(0, 100);

        for (int x = 0; x < widthX; x++)
        {
            progress = x / (float)widthX * 100;
            for (int z = 0; z < widthZ; z++)
            {
                float xCoord = x / waveLength + randomOffsetX;
                float zCoord = z / waveLength + randomOffsetZ;
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
        for (int i = 0; i < blockPrefabInfos.Length; i++)
        {
            int blockHeight = blockPrefabInfos[i].height;
            //blockPos.y = 0;

            if (blockHeight < y)
            {
                if (visible)
                {
                    GameObject block = Instantiate(blockPrefabInfos[i].block, blockPos, Quaternion.identity);

                    worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(blockPrefabInfos[i].region, visible, block, true);

                    // 생성한 블록 위에 오브젝트 설치
                    StartCoroutine(CreateObject(blockPos + new Vector3(0, 0.5f, 0), blockPrefabInfos[i].region));

                    // 몬스터 스포너 생성할 위치 저장 (1개)
                    if(blockPrefabInfos[i].region == Region.Sand && !isCreatedSpawner)
                    {
                        if (blockPos.x > widthX * 0.3 && blockPos.z > widthZ * 0.3)
                        {
                            if (Random.Range(0, 1000) >= 999)
                            {
                                isCreatedSpawner = true;
                                monsterSpawnerPos = blockPos + Vector3.up;
                            }
                        }
                    }
                    if(!isCreatedPortal)
                    {
                        if (blockPos.x > widthX * 0.3 && blockPos.z > widthZ * 0.3)
                        {
                            if (Random.Range(0, 500) >= 499)
                            {
                                isCreatedPortal = true;
                                Instantiate(portalInfo, blockPos, Quaternion.identity);
                            }
                        }
                    }
                }
                else
                {
                    // 가장자리 블럭 생성
                    if (blockPos.x == 0 || blockPos.x == widthX - 1 || blockPos.z == 0 || blockPos.z == widthZ - 1)
                    {
                        GameObject block = Instantiate(blockPrefabInfos[i].block, blockPos, Quaternion.identity);

                        worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(blockPrefabInfos[i].region, true, block, true);
                    }
                    else
                    {
                        // 보이지 않는 부분은 생성은 하지 않고 블록정보만 저장
                        worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(blockPrefabInfos[i].region, visible, null, true);
                    }
                }
                break;
            }
        }

        yield return null;
    }
    public void CheckAroundDestroyedBlock(Vector3 blockPos)
    {
        worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].isExist = false;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (!(x == 0 && y == 0 && z == 0))
                    {
                        if (blockPos.x + x < 0 || blockPos.x + x >= widthX)
                        {
                            continue;
                        }
                        if (blockPos.y + y < 0 || blockPos.y + y >= height)
                        {
                            continue;
                        }
                        if (blockPos.z + z < 0 || blockPos.z + z >= widthZ)
                        {
                            continue;
                        }
                        Vector3 neighbour = new Vector3(blockPos.x + x, blockPos.y + y, blockPos.z + z);
                        DrawBlock(neighbour);
                    }
                }
            }
        }
    }
    private void DrawBlock(Vector3 blockPos)
    {
        BlockInfo worldBlock = worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z];

        if (!worldBlock.isExist)
        {
            return;
        }

        if (!worldBlock.isVisible)
        {
            GameObject newBlock = null;

            for (int i = 0; i < blockPrefabInfos.Length; i++)
            {
                int blockHeight = blockPrefabInfos[i].height;

                if (blockHeight < blockPos.y)
                {
                    newBlock = Instantiate(blockPrefabInfos[i].block, blockPos, Quaternion.identity);
                    break;
                }
            }

            if (newBlock != null)
            {
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].block = newBlock;
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].isVisible = true;
            }
        }
    }
    IEnumerator CreateObject(Vector3 objectPos, Region region)
    {
        if (Random.Range(0, 100) >= prob_NonObject)
        {
            // 맵 영역에 맞는 오브젝트 리스트를 갖고있지 않으면 오브젝트 생성 코루틴 종료
            if (System.Array.FindIndex(envirionmentsInfos, info => info.region == region) < 0)
            {
                yield break;
            }

            // 오브젝트 타입이 현재 지역과 일치할때까지 랜덤하게 찾음
            // 만약 볼륨이 더 커지면 지역과 일치하는 오브젝트를 먼저 찾고 그 범위 내에서 랜덤으로 선택하는 로직으로 수정
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
    IEnumerator CreateSpawner(Vector3 objectPos)
    {
        Instantiate(monsterSpawnerInfo, objectPos, Quaternion.identity);

        yield break;
    }

}

