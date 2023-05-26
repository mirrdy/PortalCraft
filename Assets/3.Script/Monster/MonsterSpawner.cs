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
            // 비활성화된 몬스터를 찾아서 스폰
            for (int i = 0; i < spawnList.Length; i++)
            {
                if (!spawnList[i].activeSelf)
                {
                    spawnList[i].SetActive(true);
                    yield return null;
                }
            }

            // 비활성화된 몬스터가 없을 경우 일정 시간 대기 후 다시 시도
            yield return new WaitForSeconds(spawnTime);
        }
    }

}
