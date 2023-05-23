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

            // �̵� ������ ���� ������ ������ ��ġ ����
            Vector3 targetPosition = monster.spawnPoint + Random.insideUnitSphere * monster.patrolRange;
            targetPosition.y = monster.spawnPoint.y;
            //Debug.Log("��ġ����: " + targetPosition);

            // ���Ͱ� �÷��̾� ���� �ٶ󺸵��� ȸ�� ����
            Vector3 patrolDirection = targetPosition - monster.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(patrolDirection);
            monster.transform.rotation = targetRotation;

            // ���͸� ���õ� ��ġ�� �̵���Ŵ
            while (Vector3.Distance(monster.transform.position, targetPosition) > 0.1f)
            {
                Vector3 direction = targetPosition - monster.transform.position;
                direction.Normalize();
                Vector3 newPosition = monster.transform.position + direction * 10f * Time.deltaTime;
                monster.transform.position = newPosition;
                yield return null;
            }

            // Ÿ�� ��ġ�� �����ϸ� �ִϸ��̼� ����
            monster.animator.SetBool("isMove", false);
            yield return new WaitForSeconds(3f);
        }
    }
}
