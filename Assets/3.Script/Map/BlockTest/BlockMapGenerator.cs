using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public int type;
    public bool isVisible;
    public GameObject block;
    public Block(int type, bool isVisible, GameObject block)
    {
        this.type = type;
        this.isVisible = isVisible;
        this.block = block;
    }
}


public class BlockMapGenerator : MonoBehaviour
{
    [Header("블록")]
    public GameObject prefab_BlackBlock;
    public GameObject prefab_BrownBlock;

    public GameObject prefab_SnowBlock;
    public GameObject prefab_GroundBlock;
    public GameObject prefab_GrassBlock;

    public GameObject prefab_CoalBlock;
    public GameObject prefab_MetalBlock;
    public GameObject prefab_GoldBlock;
    public GameObject prefab_FloorBlock; // 바닥블럭(파괴불가)


    [Header("맵정보")]
    static public int widthX = 125;
    static public int widthZ = 125;
    static public int height = 125;
    public float waveLength = 0;
    public float amplitude = 0;

    public Block[,,] worldBlocks = new Block[widthX, height, widthZ];

    public float groundHeightOffset = 20;
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
        seed = (int)Random.Range(0, 100);

        for (int x = 0; x < widthX; x++)
        {
            for (int z = 0; z < widthZ; z++)
            {
                float xCoord = (x + 0) / waveLength;
                float zCoord = (z + 0) / waveLength;
                int y = (int)(Mathf.PerlinNoise(xCoord, zCoord) * amplitude + groundHeightOffset);

                Vector3 pos = new Vector3(x, y, z);
                StartCoroutine(CreateBlock(y, pos, true));

                while (y > 0)
                {
                    // 게임오브젝트는 생성하지 않고 블럭의 정보만 배열에 넣음
                    y--;
                    pos = new Vector3(x, y, z);
                    StartCoroutine(CreateBlock(y, pos, false));
                }

            }

            yield return null;
        }
    }
    IEnumerator CreateBlock(int y, Vector3 blockPos, bool visible)
    {
        if(y>40)
        {
            if(visible)
            {
                GameObject block = Instantiate(prefab_SnowBlock, blockPos, Quaternion.identity);
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(1, visible, block);
            }
            else
            {
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(1, visible, null);
            }
        }

        else if (y > 35)
        {
            if (visible)
            {
                GameObject block = Instantiate(prefab_GrassBlock, blockPos, Quaternion.identity);
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(2, visible, block);
            }
            else
            {
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(2, visible, null);
            }
        }
        else if (y > 20)
        {
            if (visible)
            {
                GameObject block = Instantiate(prefab_GroundBlock, blockPos, Quaternion.identity);
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(2, visible, block);
            }
            else
            {
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(2, visible, null);
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
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(4, true, block);
            }
            else
            {
                worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(4, true, null);
            }
            

            
        }

        if(y==0)
        {
            if(visible)
            {
                if (visible)
                {
                    GameObject block = Instantiate(prefab_FloorBlock, blockPos, Quaternion.identity);
                    worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(5, visible, block);
                }
                else
                {
                    worldBlocks[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(5, visible, null);
                }
            }
        }


        yield return null;
    }

}
