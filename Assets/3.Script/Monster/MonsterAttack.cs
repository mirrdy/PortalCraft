using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    private int damage;
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Ʈ���Ŵ� ����");
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            Debug.Log("�÷��̾����");
            other.TryGetComponent(out PlayerControl player);
            damage = Mathf.RoundToInt(GetComponentInParent<MonsterControl>().atk);
            Debug.Log(damage);
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 hitNormal = transform.position - other.transform.position;
            player.OnDamage(damage, hitPoint, hitNormal);
            
        }
    }

}
