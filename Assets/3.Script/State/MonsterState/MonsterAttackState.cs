using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackState : EntityState
{
    private bool canAttack = true;
    private WaitForSeconds waitForSeconds;
    private MonoBehaviour monoBehaviour;
    public override void EnterState(LivingEntity entity)
    {
        if (monoBehaviour == null)
        {
            monoBehaviour = entity.GetComponent<MonoBehaviour>();
        }
        waitForSeconds = new WaitForSeconds(entity.attackTime);
        if (canAttack)
        {
            entity.animator.SetBool("isAttack", true);
            entity.GetComponent<CapsuleCollider>().enabled = true;
            monoBehaviour.StartCoroutine(Attack_co(entity));
        }
       
    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isAttack", false);
        monoBehaviour.StopCoroutine(Attack_co(entity));
        entity.GetComponent<CapsuleCollider>().enabled= false;
    }

    public override void UpdateState(LivingEntity entity)
    {
        EnterState(entity);
    }
    private IEnumerator Attack_co(LivingEntity entity)
    {
        canAttack = false;
        yield return waitForSeconds;
        canAttack = true;
    }
}
