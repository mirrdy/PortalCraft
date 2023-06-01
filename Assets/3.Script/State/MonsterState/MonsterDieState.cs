using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDieState : EntityState
{
    public override void EnterState(LivingEntity entity)
    {
        if (entity.onDeath != null && entity.isDead)
        {
            entity.onDeath.Invoke();
            entity.StartCoroutine(Die_co(entity));
        }
    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isDead", false);
        entity.gameObject.SetActive(false);
    }

    public override void UpdateState(LivingEntity entity)
    {
            
    }
    private IEnumerator Die_co(LivingEntity entity)
    {
        entity.animator.SetBool("isDead", true);
        yield return new WaitForSeconds(5f);
        entity.ChangeState(new MonsterIdleState());
    }
}
