using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody rigid;
    public TrailRenderer trailEffect;

    private void Start()
    {
        TryGetComponent(out rigid);
    }

    private void Update()
    {
        transform.forward = rigid.velocity;
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Monster"))
        {
            Debug.Log("몬스터 화살 맞음");
            coll.TryGetComponent(out MonsterControl monsterControl);
            int damage = PlayerControl.instance.equipItem.TryGetComponent(out Bow bow) ? bow.attackDamage : 0;
            //Vector3 hitPoint = coll.ClosestPoint(transform.position);
            //Vector3 hitNormal = transform.position - coll.transform.position;
            monsterControl.TakeDamage(damage);      
        }
        Destroy(gameObject);
    }
}
