using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    private float damage;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            damage = Mathf.RoundToInt(GetComponent<BossControl>().atk);
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 hitNormal = transform.position - other.transform.position;
        }
    }
}
