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
        //        chunk.blocks = new Block[noiseWidth, height, noiseLength]; //ûũ ��� ������ �ʱ�ȭ
        //        chunkLoader.SetChunk(chunk);

        //        CreateWorld(chunkLoader, pos); //ûũ�� �����͸� ������� ���� ����
        //    }
        //}
    }
}
