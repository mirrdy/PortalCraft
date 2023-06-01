using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    public int weaponAttackDamage; //������ݷ�

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Debug.Log("�����ǰ�");
            other.TryGetComponent(out MonsterControl monsterControl); 
            int damage = PlayerControl.instance.staters.attack + weaponAttackDamage;
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 hitNormal = transform.position - other.transform.position;
            monsterControl.OnDamage(damage, hitPoint, hitNormal);
        }
    }
}
