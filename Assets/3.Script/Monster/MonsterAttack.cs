using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    private int damage;
    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            MonsterControl monster = GetComponentInParent<MonsterControl>();
            if (!monster.isDead&& Time.time >= monster.lastAttackTimebet + monster.timebetAttack)
            {
                monster.lastAttackTimebet = Time.time;
                //gameObject.SetActive(false);
                other.TryGetComponent(out PlayerControl player);
                damage = Mathf.RoundToInt(GetComponentInParent<MonsterControl>().atk);
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;
                player.OnDamage(damage, hitPoint, hitNormal);
            }
        }
      
    }

}
