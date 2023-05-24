using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPatrolState : EntityState
{
    private MonsterControl monster;
    private IEnumerator patrol_co;
<<<<<<< HEAD
    private bool canPatrol;
    public override void EnterState(LivingEntity entity)
    {
        patrol_co = Patrol_co();
        canPatrol = true;
=======
    public override void EnterState(LivingEntity entity)
    {
        patrol_co = Patrol_co();
>>>>>>> KY
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
<<<<<<< HEAD

        //if (canPatrol)
        //{
        //    canPatrol = false;
        //    entity.StartCoroutine(patrol_co);
        //}
=======
       
>>>>>>> KY
    }

    private IEnumerator Patrol_co()
    {
        while (true)
        {
            monster.animator.SetBool("isMove", true);

            // 이동 가능한 범위 내에서 랜덤한 위치 선택
            Vector3 targetPosition = monster.spawnPoint + Random.insideUnitSphere * monster.patrolRange;
            targetPosition.y = monster.spawnPoint.y;
<<<<<<< HEAD
=======
            //Debug.Log("위치선정: " + targetPosition);
>>>>>>> KY

            // 몬스터가 플레이어 쪽을 바라보도록 회전 설정
            Vector3 patrolDirection = targetPosition - monster.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(patrolDirection);
            monster.transform.rotation = targetRotation;

            // 몬스터를 선택된 위치로 이동시킴
<<<<<<< HEAD
            //while (Vector3.Distance(monster.transform.position, targetPosition) > 0.1f)
            //{
            //    Vector3 direction = targetPosition - monster.transform.position;
            //    //direction.y = 0f; // Y값을 0으로 설정하여 수직 이동을 방지합니다.
            //    direction.Normalize();
            //    //Vector3 newPosition = monster.transform.position + direction * monster.moveSpeed * Time.deltaTime;
            //    //monster.transform.position = newPosition;
            //    monster.entityController.Move(direction * monster.moveSpeed * Time.deltaTime);
            //    yield return null;
            //}
            while (true)
            {
                Vector3 direction = targetPosition - monster.transform.position;
                direction.y = 0f; // Y값을 0으로 설정하여 수직 이동을 방지합니다.
                if (direction.magnitude <= 0.1f)
                    break; // 거리 값이 0.1 이하인 경우 반복 종료
                direction.Normalize();
                monster.entityController.Move(direction * monster.moveSpeed * Time.deltaTime);
=======
            while (Vector3.Distance(monster.transform.position, targetPosition) > 0.1f)
            {
                Vector3 direction = targetPosition - monster.transform.position;
                direction.Normalize();
                Vector3 newPosition = monster.transform.position + direction * 10f * Time.deltaTime;
                monster.transform.position = newPosition;
>>>>>>> KY
                yield return null;
            }

            // 타겟 위치에 도착하면 애니메이션 끄기
            monster.animator.SetBool("isMove", false);
            yield return new WaitForSeconds(3f);
        }
    }
}
