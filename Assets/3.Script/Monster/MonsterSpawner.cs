using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnMonsterList;
    private GameObject[] spawnList;
    private float spawnTime = 60f;
    private int spawnRange = 10;

    private void Awake()
    {
        Debug.Log(spawnMonsterList.Length);
        spawnList = new GameObject[spawnMonsterList.Length];
        for (int i = 0; i < spawnMonsterList.Length; i++)
        {
            //MonsterControl spawnMonster = spawnList[i].gameObject.GetComponent<MonsterControl>();
            Vector3 randomSpawnPosition = transform.position + Random.insideUnitSphere * spawnRange;
            randomSpawnPosition.y = 0f;
            spawnList[i] = Instantiate(spawnMonsterList[i], randomSpawnPosition, Quaternion.identity);
            spawnList[i].gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(Spawn_co());
    }

    private IEnumerator Spawn_co()
    {
        while (true)
        {
            // ��Ȱ��ȭ�� ���͸� ã�Ƽ� ����
            for (int i = 0; i < spawnList.Length; i++)
            {
                if (!spawnList[i].activeSelf)
                {
                    spawnList[i].SetActive(true);
                    yield return null;
                }
            }

            // ��Ȱ��ȭ�� ���Ͱ� ���� ��� ���� �ð� ��� �� �ٽ� �õ�
            yield return new WaitForSeconds(spawnTime);
        }
    }

}
