using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] public GameObject[] spawnMonsterList;
    public GameObject[] spawnList;
    private Vector3[] randomSpawnPosition;
    private float spawnTime = 60f;
    private int spawnRange = 10;

    private void Awake()
    {
        spawnList = new GameObject[spawnMonsterList.Length];
        randomSpawnPosition = new Vector3[spawnMonsterList.Length];
        for (int i = 0; i < spawnMonsterList.Length; i++)
        {
            //MonsterControl spawnMonster = spawnList[i].gameObject.GetComponent<MonsterControl>();
            randomSpawnPosition[i] = transform.position + Random.insideUnitSphere * spawnRange;
            randomSpawnPosition[i].y = transform.position.y;
            spawnList[i] = Instantiate(spawnMonsterList[i], randomSpawnPosition[i], Quaternion.identity);
            spawnList[i].transform.SetParent(gameObject.transform);
            spawnList[i].gameObject.SetActive(false);
        }
    }
        
    private void OnEnable()
    {
        StartCoroutine(Spawn_co());
    }
    private void OnDisable()
    {
        StopCoroutine(Spawn_co());
    }

    private IEnumerator Spawn_co()
    {
        while (true)
        {

            // 비활성화된 몬스터를 찾아서 스폰
            for (int i = 0; i < spawnList.Length; i++)
            {
                spawnList[i].transform.position = randomSpawnPosition[i];
                if (!spawnList[i].activeSelf)
                {
                    if (Physics.Raycast(randomSpawnPosition[i] + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 3f))
                    {
                        if (hit.collider.gameObject.layer == 8)
                        {
                            randomSpawnPosition[i].y += 2f;
                            spawnList[i].transform.position = randomSpawnPosition[i];
                        }
                        #region 예전 코드(무한루프 발생)
                        /*if (hit.collider.gameObject.layer == 8 && hit.distance >= 0.5f && hit.distance <= 1.5f)
                        {
                            if (hit.distance > 0.5 && hit.distance<1.5)
                            {
                                randomSpawnPosition[i].y += 1f;
                                spawnList[i].transform.position = randomSpawnPosition[i];
                            }
                            else if(hit.distance>1.5&& hit.distance < 2.5)
                            {
                                spawnList[i].transform.position = randomSpawnPosition[i];
                            }
                            else if (hit.distance > 2.5)
                            {
                                randomSpawnPosition[i].y -= 1f;
                                spawnList[i].transform.position = randomSpawnPosition[i];
                            }
                            else
                            {
                                randomSpawnPosition[i] = transform.position + Random.insideUnitSphere * spawnRange;
                                randomSpawnPosition[i].y = transform.position.y;
                                i--;
                                continue;
                            }
                        }*/
                        #endregion
                    }
                    else
                    {
                        randomSpawnPosition[i] = transform.position + Random.insideUnitSphere * spawnRange;
                        randomSpawnPosition[i].y = transform.position.y;
                        i--;
                        continue;
                    }

                    spawnList[i].SetActive(true);
                    yield return null;
                }
            }

            // 비활성화된 몬스터가 없을 경우 일정 시간 대기 후 다시 시도
            yield return new WaitForSeconds(spawnTime);
        }
    }

}
