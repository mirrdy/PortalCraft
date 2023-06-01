using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdleState : EntityState
{
    private MonsterControl monster;

    public override void EnterState(LivingEntity entity)
    {
        monster = entity.GetComponent<MonsterControl>();
    }

    public override void UpdateState(LivingEntity entity)
    {
        if (monster.target == null) 
        {   
            entity.ChangeState(new MonsterPatrolState());
        }
        if(monster.target!=null)
        {
            entity.ChangeState(new MonsterChaseState());
        }
    }
    public override void ExitState(LivingEntity entity)
    {

    }
}
