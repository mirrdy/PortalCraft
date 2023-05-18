using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : EntityState
{
    MonsterControl monster;
    public override void EnterState(LivingEntity entity)
    {
        entity.animator.SetBool("isMove", true);
        monster = entity.GetComponentInChildren<MonsterControl>();
    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isMove", false);
    }

    public override void UpdateState(LivingEntity entity)
    {
        if (monster.target != null)
        {
            Vector3 direction = monster.target.transform.position - entity.transform.position;
            direction.Normalize();
        }
        else
        {
            entity.ChangeState(new IdleState());
        }
    }
}
