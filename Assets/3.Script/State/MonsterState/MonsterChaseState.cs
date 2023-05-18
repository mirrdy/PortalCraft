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
        Debug.Log("������Ʈ������");
        Debug.Log(monster.target);
        
        if (monster.target != null)
        {
            Vector3 direction = monster.target.transform.position - entity.transform.position;
            direction.Normalize();
            Debug.Log(direction);
            Vector3 newPosition = entity.transform.position + direction * 10 * Time.deltaTime;
            entity.transform.position = newPosition;

            // ���Ͱ� �÷��̾� ���� �ٶ󺸵��� ȸ�� ����
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            entity.transform.rotation = targetRotation;

        }
        else
        {
            entity.ChangeState(new MonsterIdleState());
        }
    }
}
