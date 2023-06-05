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

    static public int widthX = 100;
    static public int widthZ = 100;
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
    public MapObjectPrefabInfo[] envirionmentInfos;
    public int prob_NonObject; // ������Ʈ�� ������ ���� Ȯ�� (0~100)
    // ���� ������
    public GameObject monsterSpawnerInfo;
    private Vector3[] monsterSpawnerPos;
    private bool[] isCreatedSpawner;
    // ��Ż
    public GameObject portalInfo;
    private Vector3[] portalPos;
    private bool[] isCreatedPortal;
    // �÷��̾� ������
    public GameObject prefab_playerSpawner;
    private Vector3 playerSpawnerPos;
    private bool isCreatedPlayerSpawner;
    // ������
    public GameObject prefab_Deadzone;


    [Header("������")]
    public float waveLength = 0;
    public float amplitude = 0;

    private int seed;

    public BlockInfo[,,,] worldBlocks;

    public float groundHeightOffset = 20;
    public bool isFinishBlockGeneration = false;
    public float progress = 0;

    [Header("�� ��ġ")]
    public Vector3[] islandPos;
    private GameObject[] islands;

    // Start is called before the first frame update
    void Start()
    {
        CreateIsland();
        SetCollisionLayer();
        StartCoroutine(InitGame());
    }
    private void CreateIsland()
    {
        islands = new GameObject[islandPos.Length];
        for(int i=0; i<islandPos.Length; i++)
        {
            islands[i] = new GameObject($"Island{i + 1}");
            islands[i].transform.position = islandPos[i];
        }
        isCreatedPortal = new bool[islandPos.Length];
        portalPos = new Vector3[islandPos.Length];
        isCreatedSpawner = new bool[islandPos.Length];
        monsterSpawnerPos = new Vector3[islandPos.Length];
        worldBlocks = new BlockInfo[islandPos.Length, widthX, height, widthZ];
    }
    private void SetCollisionLayer()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int fieldItemLayer = LayerMask.NameToLayer("FieldItem");
        int monsterLayer = LayerMask.NameToLayer("Monster");

        Physics.IgnoreLayerCollision(fieldItemLayer, playerLayer);
        Physics.IgnoreLayerCollision(fieldItemLayer, fieldItemLayer);
        Physics.IgnoreLayerCollision(fieldItemLayer, monsterLayer);
    }
    

    IEnumerator InitGame()
    {
        //PlayerControl.instance.enabled = false;
        // �� ����
        yield return StartCoroutine(MapInit());

        // �� ���� ���
        while (true)
        {
            if (isFinishBlockGeneration)
            {
                break;
            }
            yield return null;
        }

        // �� �� ������Ʈ ����
        CreatePortal();
        CreateMonsterSpawner();
        ResetPlayer();
        PlayerControl.instance.whenPlayerDie += ResetPlayer;
        //PlayerControl.instance.enabled = true;

        // ����2
    }
    private void CreatePortal()
    {
        for(int i=0; i<islandPos.Length; i++)
        {
            if(!isCreatedPortal[i])
            {
                SetDefaultPortalPos(i);
            }
            GameObject portal = Instantiate(portalInfo);
            portal.transform.SetParent(islands[i].transform);
            portal.transform.localPosition = portalPos[i];
        }
        SetPortalLink();
    }
    private void SetDefaultPortalPos(int islandIndex)
    {
        for (int y = 0; y < height; y++)
        {
            if (worldBlocks[islandIndex, widthX / 2, y, widthZ / 2].isVisible)
            {
                portalPos[islandIndex] = worldBlocks[islandIndex, widthX / 2, y, widthZ / 2].block.transform.localPosition + Vector3.up;
            }
        }
    }
    private void SetPortalLink()
    {
        for (int i = 0; i < islandPos.Length; i++)
        {
            if (i < islandPos.Length - 1)
            {
                PortalController portal1 = islands[i].GetComponentInChildren<PortalController>();
                PortalController portal2 = islands[i + 1].GetComponentInChildren<PortalController>();

                portal1.destPortal = portal2;
                portal2.destPortal = portal1;
            }
        }
    }
    private void CreateMonsterSpawner()
    {
        for (int i = 0; i < islandPos.Length; i++)
        {
            if(!isCreatedSpawner[i])
            {
                SetDefaultMonsterSpawnerPos(i);
            }
            //GameObject spawner = Instantiate(monsterSpawnerInfo, monsterSpawnerPos[i], Quaternion.identity);
            GameObject spawner = Instantiate(monsterSpawnerInfo);
            spawner.transform.SetParent(islands[i].transform);
            spawner.transform.localPosition = monsterSpawnerPos[i];
            spawner.gameObject.SetActive(true);
        }
    }
    private void SetDefaultMonsterSpawnerPos(int islandIndex)
    {
        for (int y = 0; y < height; y++)
        {
            if (worldBlocks[islandIndex, widthX / 3, y, widthZ / 3].isVisible)
            {
                monsterSpawnerPos[islandIndex] = worldBlocks[islandIndex, widthX / 3, y, widthZ / 3].block.transform.localPosition + Vector3.up;
            }
        }
    }

    IEnumerator MapInit()
    {
        int islandCount = islandPos.Length;
        

        for (int i = 0; i < islandCount; i++)
        {
            float randomOffsetX = Random.Range(0, 100);
            float randomOffsetZ = Random.Range(0, 100);

            float totalProgressSize = ((float)widthX * islandCount);
            float progressOffset = i * (100 / islandCount);
            for (int x = 0; x < widthX; x++)
            {
                progress = (x / totalProgressSize * 100) + progressOffset;
                for (int z = 0; z < widthZ; z++)
                {
                    float xCoord = x / waveLength + randomOffsetX;
                    float zCoord = z / waveLength + randomOffsetZ;
                    int noiseValueY = (int)(Mathf.PerlinNoise(xCoord, zCoord) * amplitude + groundHeightOffset);

                    Vector3 pos = new Vector3(x, noiseValueY, z);
                    StartCoroutine(CreateBlock(i, noiseValueY, pos, true));

                    // ������ Y���� �� ��ġ �� �� �غκк��ʹ� ����� �������� �ʰ� ������ �迭�� ������
                    for (int y = noiseValueY - 1; y > 0; y--)
                    {
                        pos = new Vector3(x, y, z);
                        StartCoroutine(CreateBlock(i, y, pos, false));
                    }
                }
                yield return null;
            }
            GameObject deadzone = Instantiate(prefab_Deadzone, new Vector3(islandPos[i].x, 0, islandPos[i].z), Quaternion.identity);
            deadzone.transform.localScale = new Vector3(widthX, 1, widthZ);
        }
        islands[1].SetActive(false);
        Debug.Log("������");
        progress = 100;
        isFinishBlockGeneration = true;
    }

    IEnumerator CreateBlock(int islandIndex, int y, Vector3 blockPos, bool visible)
    {
        for (int i = 0; i < blockPrefabInfos.Length; i++)
        {
            int blockHeight = blockPrefabInfos[i].height;
            //blockPos.y = 0;

            if (blockHeight < y)
            {
                if (visible)
                {
                    //GameObject block = Instantiate(blockPrefabInfos[i].block, blockPos, Quaternion.identity);
                    GameObject block = Instantiate(blockPrefabInfos[i].block);
                    block.transform.SetParent(islands[islandIndex].transform);
                    block.transform.localPosition = blockPos;

                    worldBlocks[islandIndex, (int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(blockPrefabInfos[i].region, visible, block, true);

                    // ������ ��� ���� ������Ʈ ��ġ
                    StartCoroutine(CreateObject(islandIndex, blockPos + new Vector3(0, 0.5f, 0), blockPrefabInfos[i].region));

                    // ���� ������ ������ ��ġ ���� (���� �� �ϳ��� �Ѱ�)
                    if (blockPrefabInfos[i].region == Region.Sand && !isCreatedSpawner[islandIndex])
                    {
                        int spawnProb = (int)(widthX * widthZ * 0.3f);
                        if (Random.Range(0, spawnProb) >= spawnProb - 1)
                        {
                            isCreatedSpawner[islandIndex] = true;
                            monsterSpawnerPos[islandIndex] = blockPos + Vector3.up;
                        }
                    }
                    // ��Ż ������ ��ġ ����
                    if (!isCreatedPortal[islandIndex])
                    {
                        int spawnProb = (int)(widthX * widthZ * 0.5f);
                        if (Random.Range(0, spawnProb) >= spawnProb - 1)
                        {
                            isCreatedPortal[islandIndex] = true;
                            portalPos[islandIndex] = blockPos + Vector3.up;
                        }
                    }

                    // �÷��̾� ���� ��ġ ���� (ó�� ���� 1���� ����)
                    if (!isCreatedPlayerSpawner)
                    {
                        int spawnProb = (int)(widthX * widthZ * 0.5f);
                        if (Random.Range(0, spawnProb) >= spawnProb - 1)
                        {
                            isCreatedPlayerSpawner = true;
                            playerSpawnerPos = blockPos + Vector3.up;
                        }
                    }
                }
                else
                {
                    // �����ڸ� �� ����
                    if (blockPos.x == 0 || blockPos.x == widthX - 1 || blockPos.z == 0 || blockPos.z == widthZ - 1)
                    {
                        //GameObject block = Instantiate(blockPrefabInfos[i].block, blockPos, Quaternion.identity);
                        GameObject block = Instantiate(blockPrefabInfos[i].block);
                        block.transform.SetParent(islands[islandIndex].transform);
                        block.transform.localPosition = blockPos;

                        worldBlocks[islandIndex, (int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(blockPrefabInfos[i].region, true, block, true);
                    }
                    else
                    {
                        // ������ �ʴ� �κ��� ������ ���� �ʰ� ��������� ����
                        worldBlocks[islandIndex, (int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(blockPrefabInfos[i].region, visible, null, true);
                    }
                }
                break;
            }
        }

        yield return null;
    }
    private void ResetPlayer()
    {
        PlayerControl.instance.TryGetComponent(out CharacterController control);

        control.enabled = false;
        
        PlayerControl.instance.transform.position = playerSpawnerPos;
        PlayerControl.instance.playerData.status.currentHp = PlayerControl.instance.playerData.status.maxHp;
        PlayerControl.instance.playerData.status.currentMp = PlayerControl.instance.playerData.status.maxMp;

        control.enabled = true;
    }
    public void CheckAroundDestroyedBlock(int islandIndex, Vector3 blockPos)
    {
        worldBlocks[islandIndex, (int)blockPos.x, (int)blockPos.y, (int)blockPos.z].isExist = false;
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
                        DrawBlock(islandIndex, neighbour);
                    }
                }
            }
        }
    }
    private void DrawBlock(int islandIndex, Vector3 blockPos)
    {
        BlockInfo worldBlock = worldBlocks[islandIndex, (int)blockPos.x, (int)blockPos.y, (int)blockPos.z];

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
                    //newBlock = Instantiate(blockPrefabInfos[i].block, blockPos, Quaternion.identity);
                    newBlock = Instantiate(blockPrefabInfos[i].block);
                    newBlock.transform.SetParent(islands[islandIndex].transform);
                    newBlock.transform.localPosition = blockPos;
                    break;
                }
            }

            if (newBlock != null)
            {
                worldBlocks[islandIndex, (int)blockPos.x, (int)blockPos.y, (int)blockPos.z].block = newBlock;
                worldBlocks[islandIndex, (int)blockPos.x, (int)blockPos.y, (int)blockPos.z].isVisible = true;
            }
        }
    }
    IEnumerator CreateObject(int islandIndex, Vector3 objectPos, Region region)
    {
        if (Random.Range(0, 100) >= prob_NonObject)
        {
            // �� ������ �´� ������Ʈ ����Ʈ�� �������� ������ ������Ʈ ���� �ڷ�ƾ ����
            if (System.Array.FindIndex(envirionmentInfos, info => info.region == region) < 0)
            {
                yield break;
            }

            // ������Ʈ Ÿ���� ���� ������ ��ġ�Ҷ����� �����ϰ� ã��
            // ���� ������ �� Ŀ���� ������ ��ġ�ϴ� ������Ʈ�� ���� ã�� �� ���� ������ �������� �����ϴ� �������� ����
            int objectIndex = 0;
            while (true)
            {
                objectIndex = Random.Range(0, envirionmentInfos.Length);
                if (envirionmentInfos[objectIndex].region == region)
                {
                    break;
                }
            }

            //GameObject environment = Instantiate(envirionmentInfos[objectIndex].mapObject, objectPos, Quaternion.identity);
            GameObject environment = Instantiate(envirionmentInfos[objectIndex].mapObject);
            environment.transform.SetParent(islands[islandIndex].transform);
            environment.transform.localPosition = objectPos;
        }
        yield return null;
    }
    IEnumerator CreateSpawner(Vector3 objectPos)
    {
        Instantiate(monsterSpawnerInfo, objectPos, Quaternion.identity);

        yield break;
    }

}

