using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public void SetChunk()
    {
        //for (int x = -aroundChunkNum; x <= aroundChunkNum; x++)
        //{
        //    for (int z = -aroundChunkNum; z <= aroundChunkNum; z++)
        //    {
        //        Vector3 pos = new Vector3(width * x, 0, length * z);

        //        GameObject chunkObjClone = Instantiate(chunkObj, pos, Quaternion.identity, chunkHolder);
        //        ChunkLoader chunkLoader = chunkObjClone.GetComponent<ChunkLoader>();
        //        Chunk chunk = new Chunk();
        //        chunk.blocks = new Block[noiseWidth, height, noiseLength]; //청크 블록 데이터 초기화
        //        chunkLoader.SetChunk(chunk);

        //        CreateWorld(chunkLoader, pos); //청크의 데이터를 기반으로 월드 생성
        //    }
        //}
    }
}
