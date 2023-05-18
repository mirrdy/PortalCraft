using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackState : EntityState
{
    public override void EnterState(LivingEntity entity)
    {
        entity.animator.SetBool("isAttack", true);
    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isAttack", false);
    }

    public override void UpdateState(LivingEntity entity)
    {

    }
}
