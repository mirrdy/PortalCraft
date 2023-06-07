using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTargeting : MonoBehaviour
{
    [SerializeField]BossControl boss;
    [SerializeField] GameObject playerBlock;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boss.target = other.transform;
            playerBlock.SetActive(true);
            boss.currentHp = boss.hp;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boss.target = other.transform;

            //for (int i = 0; i <10; i++)
            //{   
            //    if (bossUseSpawner.spawnList[0] != null)
            //    {
            //        bossUseSpawner.spawnList[0].gameObject.GetComponent<MonsterControl>().target = other.transform;
            //    }
            //}
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boss.target = null;
        }
    }
}
