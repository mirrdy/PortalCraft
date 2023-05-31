using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTargeting : MonoBehaviour
{
    [SerializeField]BossControl boss;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boss.target = other.transform;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        boss.target = other.transform;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boss.target = null;
        }
    }
}
