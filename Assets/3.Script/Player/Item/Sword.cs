using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int attackDamage;
    public float attackRate;

    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    public void Use()
    {
        StopCoroutine("Swing");
        StartCoroutine("Swing");
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Monster"))
        {
            Debug.Log("몬스터피격");
            coll.TryGetComponent(out MonsterControl monsterControl);
            int damage = PlayerControl.instance.playerData.status.attack + attackDamage;
            Vector3 hitPoint = coll.ClosestPoint(transform.position);
            Vector3 hitNormal = transform.position - coll.transform.position;
            monsterControl.TakeDamage(damage);
        }
    }
}
