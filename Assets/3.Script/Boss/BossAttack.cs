using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    private int damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(other.TryGetComponent(out PlayerControl player))
            {
                damage = Mathf.RoundToInt(GetComponentInParent<BossControl>().atk);
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;
                player.OnDamage(damage, hitPoint, hitNormal);


            }
            //other.Ondamage();
        }
    }
}
