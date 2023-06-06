using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Q : MonoBehaviour
{
    private Rigidbody rigid;
    public TrailRenderer trailEffect;
    private float lifeTime;

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
            coll.TryGetComponent(out MonsterControl monsterControl);
            int damage = PlayerControl.instance.equipItem.TryGetComponent(out Bow bow) ? bow.SkillDamage_1 : 0;
            monsterControl.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (coll.CompareTag("Block") || coll.CompareTag("Environment"))
        {
            Destroy(gameObject, lifeTime);
        }
    }
}
