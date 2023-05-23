using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterReturnState : EntityState
{
    MonsterControl monster;
    //IEnumerator return_co;
    public override void EnterState(LivingEntity entity)
    {
        //return_co = Return_co(entity);
        if (monster == null)
        {
            entity.TryGetComponent(out monster);
        }
        entity.animator.SetBool("isMove", true);
        

    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isMove", false);
    }

    public override void UpdateState(LivingEntity entity)
    {
        if (monster.target == null)
        {
            Vector3 direction = monster.spawnPoint - monster.transform.position;
            direction.Normalize();
            Vector3 newPosition = monster.transform.position + direction * 10f * Time.deltaTime;
            monster.transform.position = newPosition;
            float distance = Vector3.Distance(monster.transform.position, monster.spawnPoint);
            // 몬스터가 스폰 포인트 쪽을 바라보도록 회전 설정
            Vector3 spawnDirection = monster.spawnPoint - entity.transform.position;
            spawnDirection.y = 0f; // Y 축 방향을 무시하여 평면 상의 방향만 고려합니다.
            Quaternion targetRotation = Quaternion.LookRotation(spawnDirection);
            monster.transform.rotation = targetRotation;

            if (distance<0.1f)
            {
                entity.ChangeState(new MonsterIdleState());
            }
        }
        else
        {
            entity.ChangeState(new MonsterChaseState());
        }
    }

    //private IEnumerator Return_co(LivingEntity entity)
    //{
    //    while(entity.transform.position == monster.spawnPoint)
    //    {
    //        Vector3 direction = monster.spawnPoint - monster.transform.position;
    //        direction.Normalize();
    //        Vector3 newPosition = monster.spawnPoint + direction * 10f * Time.deltaTime;
    //        monster.transform.position = newPosition;
    //    }
    //    yield return null;
    //    entity.ChangeState(new MonsterIdleState());
    //}
}
