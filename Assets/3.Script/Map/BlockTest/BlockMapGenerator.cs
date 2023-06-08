using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using System.IO;

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

    [Header("블록")]
    public BlockPrefabInfo[] blockPrefabInfos;



    [Header("환경 오브젝트")]
    public MapObjectPrefabInfo[] envirionmentInfos;
    public int prob_NonObject; // 오브젝트가 생기지 않을 확률 (0~100)
    // 몬스터 스포너
    public GameObject[] monsterSpawnerInfo;
    private Vector3[] monsterSpawnerPos;
    private bool[] isCreatedSpawner;
    // 포탈
    public GameObject[] portalInfo;
    private Vector3[] portalPos;
    private bool[] isCreatedPortal;
    // 포탈 - NPC
    public GameObject prefab_PortalNPC;

    // 플레이어 스포너
    public GameObject prefab_PlayerSpawner;
    public Vector3 playerSpawnerPos;
    private bool isCreatedPlayerSpawner;

    // 제작대
    public GameObject prefab_CraftingTable;
    private Vector3 craftingTablePos;
    private bool isCreatedCraftingTable;

    // 데드존
    public GameObject prefab_Deadzone;


    [Header("맵정보")]
    public float waveLength = 0;
    public float amplitude = 0;

    private int seed;

    //public BlockInfo[,,,] worldBlocks;

    public float groundHeightOffset = 20;
    public bool isFinishBlockGeneration = false;
    public float progress = 0;

    [Header("섬 위치")]
    public Vector3[] islandPos;
    private GameObject[] islands;
    GameObject bossMap;

    public MapData mapData;

    private bool isExistSaveFile;

    // Start is called before the first frame update
    void Start()
    {
        CreateIsland();
        SetCollisionLayer();
        StartCoroutine(InitGame_co());
    }

    private void CreateIsland()
    {
        mapData = new MapData();
        mapData.list_IslandData = new List<IslandData>();

        islands = new GameObject[islandPos.Length];
        for(int i=0; i<islandPos.Length; i++)
        {
            mapData.list_IslandData.Add(new IslandData());
            mapData.list_IslandData[i].list_BlockData = new List<BlockData>();
            mapData.list_IslandData[i].list_PortalData = new List<PortalData>();
            mapData.list_IslandData[i].list_StructureData = new List<StructureData>();

            islands[i] = new GameObject($"Island{i + 1}");
            islands[i].transform.position = islandPos[i];
        }
        isCreatedPortal = new bool[islandPos.Length];
        portalPos = new Vector3[islandPos.Length];
        isCreatedSpawner = new bool[islandPos.Length];
        monsterSpawnerPos = new Vector3[islandPos.Length];

        //worldBlocks = new BlockInfo[islandPos.Length, widthX, height, widthZ];
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
    
    IEnumerator InitGame_co()
    {
        //PlayerControl.instance.enabled = false;
        // 맵 생성
        yield return StartCoroutine(MapInit_co());



        // 맵 생성 대기
        while (true)
        {
            if (isFinishBlockGeneration)
            {
                break;
            }
            yield return null;
        }

        // 맵 위 오브젝트 생성
        CreatePortal();
        CreateMonsterSpawner();
        //SetActiveIslands(); // 플레이어 위치 저장에 관한 내용 필요
        ResetPlayer();
        //InitSetCraftingTable();
        PlayerControl.instance.whenPlayerDie += ResetPlayer;
        //PlayerControl.instance.enabled = true;
        // 생성2
        bossMap.SetActive(false);
        islands[1].SetActive(false);
    }
    private void InitSetCraftingTable()
    {
        int startCheckPointX = widthX / 8;
        int endCheckPointX = widthX - (widthX / 8);
        int startCheckPointZ = widthZ / 8;
        int endCheckPointZ = widthZ - (widthZ / 8);

        while (!isCreatedCraftingTable)
        {
            // 선택한 범위에 맞는 블럭 탐색
            List<BlockData> list_SelectedForX = mapData.list_IslandData[0].list_BlockData.FindAll(
                block => (
                ((block.x >= startCheckPointX) && (block.x < endCheckPointX)) &&
                ((block.z >= startCheckPointZ) && (block.z < endCheckPointZ)) &&
                ((block.y >= groundHeightOffset) && (block.y < height)) &&
                block.isVisible && !(new Vector3(block.x, block.y, block.z).Equals(playerSpawnerPos))
                )
                );

            int randomIndex = Random.Range(0, list_SelectedForX.Count);

            craftingTablePos = new Vector3(list_SelectedForX[randomIndex].x, list_SelectedForX[randomIndex].y+0.5f, list_SelectedForX[randomIndex].z);
            isCreatedCraftingTable = true;

            /*for (int x = startCheckPointX; x < endCheckPointX; x++)
            {
                for (int z = startCheckPointZ; z < endCheckPointZ; z++)
                {
                    for (int y = (int)groundHeightOffset; y < height; y++)
                    {
                        if (worldBlocks[0, x, y, z].isVisible && !new Vector3(x, y, z).Equals(playerSpawnerPos))
                        {
                            craftingTablePos = new Vector3(x, y + 0.5f, z);
                            isCreatedCraftingTable = true;
                            break;
                        }
                    }
                    if (isCreatedCraftingTable)
                    {
                        break;
                    }
                }
                if (isCreatedCraftingTable)
                {
                    break;
                }
            }*/
        }
        GameObject craftingTable = Instantiate(prefab_CraftingTable);
        craftingTable.transform.SetParent(islands[0].transform);
        craftingTable.transform.localPosition = craftingTablePos;

    }
    private void CreatePortal()
    {
        for(int i=0; i<islandPos.Length; i++)
        {
            if(!isCreatedPortal[i])
            {
                SetDefaultPortalPos(i);
            }

            GameObject portal = Instantiate(portalInfo[i]);
            portal.transform.SetParent(islands[i].transform);
            portal.transform.localPosition = portalPos[i];
            if(i==1)
            {
                portalPos[i] = GetRandomBlockPos(i, 20);

                portal = Instantiate(portalInfo[0]);
                portal.transform.SetParent(islands[i].transform);
                portal.transform.localPosition = portalPos[i];
            }

            GameObject portalNPC = Instantiate(prefab_PortalNPC);
            portalNPC.transform.SetParent(islands[i].transform);
            if(portalNPC.TryGetComponent(out PortalNPC npc))
            {
                npc.portal = portal.GetComponent<PortalController>();
                npc.portalIndex = i;
            }

            System.Random random = new System.Random();

            List<BlockData> selectedBlocks = mapData.list_IslandData[i].list_BlockData.FindAll(
                block =>
                block.x >= portalPos[i].x - 2 && block.x < portalPos[i].x + 3 &&
                block.y >= portalPos[i].y - 2 && block.y < portalPos[i].y + 3 &&
                block.z >= portalPos[i].z - 2 && block.z < portalPos[i].z + 3 &&
                block.isVisible);

            BlockData selectedBlock = selectedBlocks[Random.Range(0, selectedBlocks.Count)];
            Vector3 NPCPos = new Vector3(selectedBlock.x + 0.5f, selectedBlock.y + 0.66f, selectedBlock.z);
            portalNPC.transform.localPosition = NPCPos;

            /*// 만약 1만번 돌때까지 NPC 위치 못잡으면 생성 안됨 - 무한루프시 프리징 문제, 예외처리 추후 필요
            for (int k=0; k<10000; k++)
            { 
                int offsetX = random.Next(-2, 3);
                int offsetY = random.Next(-2, 3);
                int offsetZ = random.Next(-2, 3);

                Vector3 offset = new Vector3(portalPos[i].x + offsetX, portalPos[i].y + offsetY, portalPos[i].z + offsetZ);
                if(portalPos[i].Equals(offset) || 
                    offset.x >= widthX || offset.x < 0 ||
                    offset.y >= height || offset.y < 0 ||
                    offset.z >= widthZ || offset.z < 0)
                {
                    continue;
                }

                if (worldBlocks[i, (int)offset.x, (int)offset.y, (int)offset.z].isVisible == true)
                {
                    Vector3 NPCPos = offset + new Vector3(0.5f, 0.66f);
                    portalNPC.transform.localPosition = NPCPos;
                    break;
                }
            }*/
        }
        bossMap = GameObject.Find("BossMap");
        if(bossMap != null)
        {
            Vector3 bossPortalPos = bossMap.transform.Find("BossMapPortal").localPosition;

            GameObject portal = Instantiate(portalInfo[1]);
            portal.transform.SetParent(bossMap.transform);
            portal.transform.localPosition = bossPortalPos + Vector3.up;
        }
        
        SetPortalLink();

    }

    private void SetDefaultPortalPos(int islandIndex)
    {
        /*for (int y = 0; y < height; y++)
        {
            if (worldBlocks[islandIndex, widthX / 2, y, widthZ / 2].isVisible)
            {
                portalPos[islandIndex] = worldBlocks[islandIndex, widthX / 2, y, widthZ / 2].block.transform.localPosition + Vector3.up;
            }
        }*/

        // 임시
        portalPos[islandIndex] = GetRandomBlockPos(islandIndex, 20);        
    }
    private Vector3 GetRandomBlockPos(int islandIndex, int offset)
    {
        int startCheckPointX = widthX / offset;
        int endCheckPointX = widthX - (widthX / offset);
        int startCheckPointZ = widthZ / offset;
        int endCheckPointZ = widthZ - (widthZ / offset);

        System.Random random = new System.Random();
        //////////////////////////////////////////////////////////////////////////////////////////////
        List<BlockData> selectedBlocks = mapData.list_IslandData[islandIndex].list_BlockData.FindAll(
                block =>
                block.x >= startCheckPointX && block.x < endCheckPointX &&
                block.y >= groundHeightOffset && block.y < height &&
                block.z >= startCheckPointZ - 2 && block.z < endCheckPointZ &&
                block.isVisible);

        BlockData selectedBlock = selectedBlocks[Random.Range(0, selectedBlocks.Count)];
        return new Vector3(selectedBlock.x, selectedBlock.y + 1, selectedBlock.z);


        /*while (true)
        {
            for (int x = startCheckPointX; x < endCheckPointX; x++)
            {
                for (int z = startCheckPointZ; z < endCheckPointZ; z++)
                {
                    for (int y = (int)groundHeightOffset; y < height; y++)
                    {
                        if (random.Next(Mathf.Abs(endCheckPointX - startCheckPointX)) < 1 && worldBlocks[islandIndex, x, y, z].isVisible)
                        {
                            return new Vector3(x, y+1, z);
                        }
                    }
                }
            }
        }*/
    }
    private void SetPortalLink()
    {
        PortalController portal1 = islands[0].transform.Find("Portal1(Clone)").GetComponent<PortalController>();
        PortalController portal2 = islands[1].transform.Find("Portal1(Clone)").GetComponent<PortalController>();

        portal1.destPortal = portal2;
        portal2.destPortal = portal1;

        portal1.gameObject.SetActive(false);
        portal2.gameObject.SetActive(false);

        portal1 = islands[1].transform.Find("Portal2(Clone)").GetComponent<PortalController>();
        portal2 = GameObject.Find("BossMap").transform.Find("Portal2(Clone)").GetComponent<PortalController>();

        portal1.destPortal = portal2;
        portal2.destPortal = portal1;

        portal1.gameObject.SetActive(false);
        portal2.gameObject.SetActive(false);
    }
    private void CreateMonsterSpawner()
    {
        for (int i = 0; i < islandPos.Length; i++)
        {
            if(!isCreatedSpawner[i])
            {
                SetDefaultMonsterSpawnerPos(i);
                monsterSpawnerPos[i] = GetRandomBlockPos(i, 3);
            }
            //GameObject spawner = Instantiate(monsterSpawnerInfo, monsterSpawnerPos[i], Quaternion.identity);
            GameObject spawner = Instantiate(monsterSpawnerInfo[i]);
            spawner.transform.SetParent(islands[i].transform);
            spawner.transform.localPosition = monsterSpawnerPos[i];
            spawner.gameObject.SetActive(true);
        }
    }
    private void SetActiveIslands()
    {
        
    }
    private void SetDefaultMonsterSpawnerPos(int islandIndex)
    {
       /* for (int y = 0; y < height; y++)
        {
            if (worldBlocks[islandIndex, widthX / 3, y, widthZ / 3].isVisible)
            {
                monsterSpawnerPos[islandIndex] = worldBlocks[islandIndex, widthX / 3, y, widthZ / 3].block.transform.localPosition + Vector3.up;
            }
        }*/
    }

    IEnumerator MapInit_co()
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

                    if(noiseValueY >= height)
                    {
                        z--;
                        continue;
                    }

                    Vector3 pos = new Vector3(x, noiseValueY, z);
                    StartCoroutine(CreateBlock_co(i, pos, true));

                    // 노이즈 Y값에 블럭 설치 후 그 밑부분부터는 블록을 생성하지 않고 정보만 배열에 저장함
                    for (int y = noiseValueY - 1; y > 0; y--)
                    {
                        pos = new Vector3(x, y, z);
                        StartCoroutine(CreateBlock_co(i, pos, false));
                    }
                }
                yield return null;
            }
            GameObject deadzone = Instantiate(prefab_Deadzone, new Vector3(islandPos[i].x, -50, islandPos[i].z), Quaternion.identity);
            deadzone.transform.localScale = new Vector3(widthX, 1, widthZ);
        }
        Debug.Log("생성끝");
        progress = 100;
        isFinishBlockGeneration = true;
    }
    IEnumerator MapInitWithSaveData_co()
    {
        int islandCount = islandPos.Length;

        progress = 0;

        for (int i = 0; i < islandCount; i++)
        {
            IslandData island = mapData.list_IslandData[i];
            List<BlockData> visibleBlocks = island.list_BlockData.FindAll(block => block.isVisible);

            int visibleBlockCount = visibleBlocks.Count;
            float totalProgressSize = ((float)visibleBlockCount * islandCount);
            float progressOffset = i * (100 / islandCount);

            for (int j=0; j<visibleBlockCount; j++)
            {
                progress = (j / totalProgressSize * 100) + progressOffset;

                Vector3 pos = new Vector3(visibleBlocks[j].x, visibleBlocks[j].y, visibleBlocks[j].z);
                StartCoroutine(CreateBlock_co(i, pos, true));

                for (int y = (int)pos.y - 1; y > 0; y--)
                {
                    pos = new Vector3(pos.x, y, pos.z);
                    StartCoroutine(CreateBlock_co(i, pos, false));
                }
                yield return null;
            }
            GameObject deadzone = Instantiate(prefab_Deadzone, new Vector3(islandPos[i].x, 0, islandPos[i].z), Quaternion.identity);
            deadzone.transform.localScale = new Vector3(widthX, 1, widthZ);
        }
        Debug.Log("생성끝");
        progress = 100;
        isFinishBlockGeneration = true;
    }

    IEnumerator CreateBlock_co(int islandIndex, Vector3 blockPos, bool visible)
    {
        for (int i = 0; i < blockPrefabInfos.Length; i++)
        {
            int blockHeight = blockPrefabInfos[i].height;
            //blockPos.y = 0;

            if (blockHeight < blockPos.y)
            {
                if (visible)
                {
                    //GameObject block = Instantiate(blockPrefabInfos[i].block, blockPos, Quaternion.identity);
                    GameObject block = Instantiate(blockPrefabInfos[i].block);
                    block.transform.SetParent(islands[islandIndex].transform);
                    block.transform.localPosition = blockPos;
                    if(block.TryGetComponent(out BlockObject blockInfo))
                    {
                        blockInfo.isCreatedByGenerator = true;
                    }

                    IslandData island = mapData.list_IslandData[islandIndex];
                    island.list_BlockData.Add(new BlockData(blockPos, i, visible, true));
                    
                    //worldBlocks[islandIndex, (int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(blockPrefabInfos[i].region, visible, block, true);

                    // 생성한 블록 위에 오브젝트 설치
                    StartCoroutine(CreateObject_co(islandIndex, blockPos + new Vector3(0, 0.5f, 0), blockPrefabInfos[i].region));

                    // 몬스터 스포너 생성할 위치 저장 (현재 섬 하나에 한개)
                    if (blockPrefabInfos[i].region == Region.Sand && !isCreatedSpawner[islandIndex])
                    {
                        if (blockPos.x > 10 && blockPos.z > 10)
                        {
                            int spawnProb = (int)(widthX * widthZ * 0.3f);
                            if (Random.Range(0, spawnProb) >= spawnProb - 1)
                            {
                                isCreatedSpawner[islandIndex] = true;
                                monsterSpawnerPos[islandIndex] = blockPos + Vector3.up;
                            }
                        }
                    }

                    /*// 포탈 생성할 위치 저장
                    if (!isCreatedPortal[islandIndex])
                    {
                        if(blockPos.x > 10 && blockPos.z > 10)
                        {
                            int spawnProb = (int)(widthX * widthZ * 0.5f);
                            if (Random.Range(0, spawnProb) >= spawnProb - 1)
                            {
                                isCreatedPortal[islandIndex] = true;
                                portalPos[islandIndex] = blockPos + Vector3.up;
                            }
                        }
                    }*/

                    // 플레이어 스폰 위치 저장 (처음 섬에 1개만 생성)
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
                    // 가장자리 블럭 생성
                    if (blockPos.x == 0 || blockPos.x == widthX - 1 || blockPos.z == 0 || blockPos.z == widthZ - 1)
                    {
                        //GameObject block = Instantiate(blockPrefabInfos[i].block, blockPos, Quaternion.identity);
                        GameObject block = Instantiate(blockPrefabInfos[i].block);
                        block.transform.SetParent(islands[islandIndex].transform);
                        block.transform.localPosition = blockPos;
                        if (block.TryGetComponent(out BlockObject blockInfo))
                        {
                            blockInfo.isCreatedByGenerator = true;
                        }
                        
                        IslandData island = mapData.list_IslandData[islandIndex];
                        island.list_BlockData.Add(new BlockData(blockPos, i, true, true));

                        //worldBlocks[islandIndex, (int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(blockPrefabInfos[i].region, true, block, true);
                    }
                    else
                    {
                        // 보이지 않는 부분은 생성은 하지 않고 블록정보만 저장

                        IslandData island = mapData.list_IslandData[islandIndex];
                        island.list_BlockData.Add(new BlockData(blockPos, i, visible, true));

                        //worldBlocks[islandIndex, (int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new BlockInfo(blockPrefabInfos[i].region, visible, null, true);
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
        if(!islands[0].activeSelf)
        {
            islands[0].SetActive(true);
            islands[1].SetActive(false);
            bossMap.SetActive(false);
        }
        control.enabled = false;

        int startCheckPointX = widthX / 8;
        int endCheckPointX = widthX - (widthX / 8);
        int startCheckPointZ = widthZ / 8;
        int endCheckPointZ = widthZ - (widthZ / 8);

        if(!isCreatedPlayerSpawner)
        {
            // 선택한 범위에 맞는 블럭 탐색
            List<BlockData> list_SelectedForX = mapData.list_IslandData[0].list_BlockData.FindAll(
                block => (
                ((block.x >= startCheckPointX) && (block.x < endCheckPointX)) &&
                ((block.z >= startCheckPointZ) && (block.z < endCheckPointZ)) &&
                ((block.y >= groundHeightOffset) && (block.y < height)) &&
                block.isVisible
                )
                );

            int randomIndex = Random.Range(0, list_SelectedForX.Count);

            playerSpawnerPos = new Vector3(list_SelectedForX[randomIndex].x, list_SelectedForX[randomIndex].y, list_SelectedForX[randomIndex].z);
            isCreatedPlayerSpawner = true;

            /*for(int x=startCheckPointX; x < endCheckPointX; x++)
            {
                for(int z = startCheckPointZ; z<endCheckPointZ; z++)
                {
                    for(int y = (int)groundHeightOffset; y<height; y++)
                    {
                        if(worldBlocks[0, x, y, z].isVisible)
                        {
                            playerSpawnerPos = new Vector3(x, y + 1, z);
                            isCreatedPlayerSpawner = true;
                            break;
                        }
                    }
                    if(isCreatedPlayerSpawner)
                    {
                        break;
                    }
                }
                if(isCreatedPlayerSpawner)
                {
                    break;
                }
            }*/
        }
        PlayerControl.instance.playerData.mapIndex = 0;

        PlayerControl.instance.transform.position = playerSpawnerPos;
        PlayerControl.instance.playerData.status.currentHp = PlayerControl.instance.playerData.status.maxHp;
        PlayerControl.instance.playerData.status.currentMp = PlayerControl.instance.playerData.status.maxMp;

        control.enabled = true;
    }
    public void CheckAroundDestroyedBlock(int islandIndex, Vector3 blockPos)
    {
        BlockData block = mapData.list_IslandData[islandIndex].list_BlockData.Find(block => new Vector3(block.x, block.y, block.z).Equals(blockPos));
        block.isExist = false;
        block.isVisible = false;

        //worldBlocks[islandIndex, (int)blockPos.x, (int)blockPos.y, (int)blockPos.z].isExist = false;
        //worldBlocks[islandIndex, (int)blockPos.x, (int)blockPos.y, (int)blockPos.z].isVisible = false;
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
        BlockData block = mapData.list_IslandData[islandIndex].list_BlockData.Find(block => new Vector3(block.x, block.y, block.z).Equals(blockPos));

        if(!block.isExist)
        {
            return;
        }
        if(!block.isVisible)
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
                    if (newBlock.TryGetComponent(out BlockObject blockInfo))
                    {
                        blockInfo.isCreatedByGenerator = true;
                    }
                    block.blockType = i;
                    block.isVisible = true;
                    break;
                }
            }
        }


        /*BlockInfo worldBlock = worldBlocks[islandIndex, (int)blockPos.x, (int)blockPos.y, (int)blockPos.z];

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
                    if (newBlock.TryGetComponent(out BlockObject blockInfo))
                    {
                        blockInfo.isCreatedByGenerator = true;
                    }
                    break;
                }
            }

            if (newBlock != null)
            {
                worldBlocks[islandIndex, (int)blockPos.x, (int)blockPos.y, (int)blockPos.z].block = newBlock;
                worldBlocks[islandIndex, (int)blockPos.x, (int)blockPos.y, (int)blockPos.z].isVisible = true;
            }
        }*/
    }
    IEnumerator CreateObject_co(int islandIndex, Vector3 objectPos, Region region)
    {
        if (Random.Range(0, 100) >= prob_NonObject)
        {
            // 맵 영역에 맞는 오브젝트 리스트를 갖고있지 않으면 오브젝트 생성 코루틴 종료
            if (System.Array.FindIndex(envirionmentInfos, info => info.region == region) < 0)
            {
                yield break;
            }

            // 오브젝트 타입이 현재 지역과 일치할때까지 랜덤하게 찾음
            // 만약 볼륨이 더 커지면 지역과 일치하는 오브젝트를 먼저 찾고 그 범위 내에서 랜덤으로 선택하는 로직으로 수정
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
}

public enum BlockType
{
    Snow = 0,   //눈
    Grass,  //풀
    Sand,   //모래
    Black,  //검정
    Brown,  //갈색
    Coal,   //석탄
    Gold,   //금
    Metal,  //철
    Rando   //랜덤
}

[System.Serializable]
public class MapData  // 플레이어 데이터 관리 클레스
{
    [XmlElement]
    public List<IslandData> list_IslandData;

    public MapData()
    {

    }
}

[System.Serializable]
public class IslandData
{
    [XmlElement]
    public List<BlockData> list_BlockData;
    [XmlElement]
    public List<PortalData> list_PortalData;
    [XmlElement]
    public List<StructureData> list_StructureData;

    public IslandData()
    {

    }
}

[System.Serializable]
public struct BlockData
{
    [XmlElement]
    public float x;
    [XmlElement]
    public float y;
    [XmlElement]
    public float z;
    [XmlElement]
    public int blockType;
    [XmlElement]
    public bool isVisible;
    [XmlElement]
    public bool isExist;
    
    

    public BlockData(float x, float y, float z, int blockType, bool isVisible, bool isExist)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.blockType = blockType;
        this.isVisible = isVisible;
        this.isExist = isExist;
    }
    public BlockData(Vector3 pos, int blockType, bool isVisible, bool isExist)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
        this.blockType = blockType;
        this.isVisible = isVisible;
        this.isExist = isExist;
    }
}

[System.Serializable]
public struct PortalData
{
    [XmlElement]
    public float x;
    [XmlElement]
    public float y;
    [XmlElement]
    public float z;
    [XmlElement]
    public int portalType;

  
    public PortalData(float x, float y, float z, int portalType)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.portalType = portalType;
    }
    public PortalData(Vector3 pos, int portalType)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
        this.portalType = portalType;
    }
}

[System.Serializable]
public struct StructureData
{
    [XmlElement]
    public float x;
    [XmlElement]
    public float y;
    [XmlElement]
    public float z;

   
    public StructureData(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public StructureData(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }
}


