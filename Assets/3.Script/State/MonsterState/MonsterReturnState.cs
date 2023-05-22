using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterReturnState : EntityState
{
    MonsterControl monster;
    IEnumerator return_co;
    public override void EnterState(LivingEntity entity)
    {
        return_co = Return_co(entity);
        if (monster == null)
        {
            entity.TryGetComponent(out monster);
        }
        entity.animator.SetBool("isMove", true);
        entity.StartCoroutine(return_co);

    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isMove", false);
        entity.StartCoroutine(return_co);
    }

    public override void UpdateState(LivingEntity entity)
    {
        
    }

    private IEnumerator Return_co(LivingEntity entity)
    {
        while(entity.transform.position == monster.spawnPoint)
        {
            Vector3 direction = monster.spawnPoint - monster.transform.position;
            direction.Normalize();
            Vector3 newPosition = monster.spawnPoint + direction * 10f * Time.deltaTime;
            monster.transform.position = newPosition;
        }
        yield return null;
        entity.ChangeState(new MonsterIdleState());
    }
}
