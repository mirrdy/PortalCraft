using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public int width;
    public int length;
    public int aroundChunkNum;
    public GameObject chunkObj;
    

    public void SetChunk()
    {
        for (int x = -aroundChunkNum; x <= aroundChunkNum; x++)
        {
            for (int z = -aroundChunkNum; z <= aroundChunkNum; z++)
            {
                Vector3 pos = new Vector3(width * x, 0, length * z);
                
               
            }
        }
    }
}
