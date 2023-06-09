using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseState : EntityState
{
    MonsterControl monster;
    public override void EnterState(LivingEntity entity)
    {
        entity.animator.SetBool("isMove", true);
        monster = entity.GetComponent<MonsterControl>();
    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isMove", false);
    }

    public override void UpdateState(LivingEntity entity)   
    {
        if (monster.isHit)
        {
            monster.target = PlayerControl.instance.transform;
        }
        if (monster.target != null)
        {   

            Vector3 targetPosition = monster.target.transform.position;
            targetPosition.y = entity.transform.position.y;
            Vector3 direction = targetPosition - entity.transform.position;
            direction.Normalize();
            float distance = Vector3.Distance(monster.transform.position, targetPosition);
            //entity.transform.position += direction * 10 * Time.deltaTime;
            if (monster.entityController.enabled)
            {
                monster.entityController.Move(direction * monster.moveSpeed * Time.deltaTime);
            }
            // 몬스터가 플레이어 쪽을 바라보도록 회전 설정
            Vector3 playerDirection = monster.target.position - entity.transform.position;
            playerDirection.y = 0; // Y 축 방향을 무시하여 평면 상의 방향만 고려합니다.
            Quaternion targetRotation = Quaternion.LookRotation(playerDirection);
            monster.transform.rotation = targetRotation;
            if(distance <= monster.attackRange)
            {
                entity.ChangeState(new MonsterAttackState());
                return;
            }

        }
        if (monster.target == null)
        {
            entity.ChangeState(new MonsterReturnState());
        }
    }
}
