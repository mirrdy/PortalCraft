using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPatrolState : EntityState
{
    private MonsterControl monster;
    private IEnumerator patrol_co;
    Vector3 targetPosition;
    //private bool canPatrol;
    public override void EnterState(LivingEntity entity)
    {
        patrol_co = Patrol_co();
        //canPatrol = true;
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

        // ���Ͱ� Ÿ�� ���� �ٶ󺸵��� ȸ�� ����
       
        //if (canPatrol)
        //{
        //    canPatrol = false;
        //    entity.StartCoroutine(patrol_co);
        //}
    }

    private IEnumerator Patrol_co()
    {
        while (true)
        {
            monster.animator.SetBool("isMove", true);

            // �̵� ������ ���� ������ ������ ��ġ ����
            Vector3 targetPosition = monster.spawnPoint + Random.insideUnitSphere * monster.patrolRange;
            targetPosition.y = monster.spawnPoint.y;

            // ���Ͱ� Ÿ�� ���� �ٶ󺸵��� ȸ�� ����
            Vector3 patrolDirection = targetPosition - monster.transform.position;
            patrolDirection.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(patrolDirection);
            monster.transform.rotation = targetRotation;

            // ���͸� ���õ� ��ġ�� �̵���Ŵ
            //while (Vector3.Distance(monster.transform.position, targetPosition) > 0.1f)
            //{
            //    Vector3 direction = targetPosition - monster.transform.position;
            //    //direction.y = 0f; // Y���� 0���� �����Ͽ� ���� �̵��� �����մϴ�.
            //    direction.Normalize();
            //    //Vector3 newPosition = monster.transform.position + direction * monster.moveSpeed * Time.deltaTime;
            //    //monster.transform.position = newPosition;
            //    monster.entityController.Move(direction * monster.moveSpeed * Time.deltaTime);
            //    yield return null;
            //}
            while (true)
            {
                targetPosition.y = monster.transform.position.y;
                Vector3 direction = targetPosition - monster.transform.position;
                direction.y = 0; // Y���� ���� ��ġ�� �����Ͽ� ���� �̵��� �����մϴ�.
                if (direction.magnitude <= 0.1f)
                {
                    break; // �Ÿ� ���� 0.1 ������ ��� �ݺ� ����
                }
                direction.Normalize();
                monster.entityController.Move(direction * monster.moveSpeed * Time.deltaTime);
                yield return null;
            }

            // Ÿ�� ��ġ�� �����ϸ� �ִϸ��̼� ����
            monster.animator.SetBool("isMove", false);
            yield return new WaitForSeconds(3f);
        }
    }
}
