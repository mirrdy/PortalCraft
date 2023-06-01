using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    public int weaponAttackDamage; //무기공격력

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Debug.Log("몬스터피격");
            other.TryGetComponent(out MonsterControl monsterControl); 
            int damage = PlayerControl.instance.staters.attack + weaponAttackDamage;
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 hitNormal = transform.position - other.transform.position;
            monsterControl.OnDamage(damage, hitPoint, hitNormal);
        }
    }
}
