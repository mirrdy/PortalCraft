using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPatrolState : EntityState
{
    private MonsterControl monster;
    private IEnumerator patrol_co;
    public override void EnterState(LivingEntity entity)
    {
        patrol_co = Patrol_co();
        if (monster == null)
        {
            entity.TryGetComponent(out monster);
        }
        entity.StartCoroutine(patrol_co);
    }

    public override void ExitState(LivingEntity entity)
    {
        monster.animator.SetBool("isMove", false);
        entity.StopCoroutine(patrol_co);
        //monster.target
    }

    public override void UpdateState(LivingEntity entity)
    {
        if (monster.target != null)
        {
            entity.ChangeState(new MonsterChaseState());
        }
       
    }

    private IEnumerator Patrol_co()
    {
        while (true)
        {
            monster.animator.SetBool("isMove", true);

            // 이동 가능한 범위 내에서 랜덤한 위치 선택
            Vector3 targetPosition = monster.spawnPoint + Random.insideUnitSphere * monster.patrolRange;
            targetPosition.y = monster.spawnPoint.y;
            //Debug.Log("위치선정: " + targetPosition);

            // 몬스터가 플레이어 쪽을 바라보도록 회전 설정
            Vector3 patrolDirection = targetPosition - monster.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(patrolDirection);
            monster.transform.rotation = targetRotation;

            // 몬스터를 선택된 위치로 이동시킴
            while (Vector3.Distance(monster.transform.position, targetPosition) > 0.1f)
            {
                Vector3 direction = targetPosition - monster.transform.position;
                direction.Normalize();
                Vector3 newPosition = monster.transform.position + direction * 10f * Time.deltaTime;
                monster.transform.position = newPosition;
                yield return null;
            }

            // 타겟 위치에 도착하면 애니메이션 끄기
            monster.animator.SetBool("isMove", false);
            yield return new WaitForSeconds(3f);
        }
    }
}
