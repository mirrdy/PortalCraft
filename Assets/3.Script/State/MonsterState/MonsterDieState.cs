using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : EntityState
{
    public override void EnterState(LivingEntity entity)
    {
        if (entity.onDeath != null && entity.isDead)
        {
            entity.onDeath.Invoke();
        }
    }

    public override void ExitState(LivingEntity entity)
    {
        
    }

    public override void UpdateState(LivingEntity entity)
    {
        
    }
}
