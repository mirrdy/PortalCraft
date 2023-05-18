using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPatrolState : EntityState
{
    public override void EnterState(LivingEntity entity)
    {
        entity.animator.SetTrigger("isMove");
    }

    public override void ExitState(LivingEntity entity)
    {
        
    }

    public override void UpdateState(LivingEntity entity)
    {
        //entity.transform.position = Vector3.MoveTowards()
    }

}
