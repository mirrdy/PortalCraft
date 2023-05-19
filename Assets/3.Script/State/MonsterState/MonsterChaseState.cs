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
        monster.target = null;
    }

    public override void UpdateState(LivingEntity entity)
    {
        
        if (monster.target != null)
        {   
            Vector3 targetPosition = monster.target.transform.position;
            targetPosition.y = entity.transform.position.y;
            Vector3 direction = targetPosition - entity.transform.position;
            direction.Normalize();
            //Debug.Log(direction);


            Vector3 newPosition = entity.transform.position + direction * 1 * Time.deltaTime;

            Debug.Log($"Ÿ�� ������: {targetPosition}");
            Debug.Log($"���� ������: {entity.transform.position}");
            Debug.Log($"�̵��� ������: {newPosition - entity.transform.position}");

            //entity.transform.position = newPosition;
            entity.transform.position += direction * 10 * Time.deltaTime;
            

            // ���Ͱ� �÷��̾� ���� �ٶ󺸵��� ȸ�� ����
            Vector3 playerDirection = monster.target.position - entity.transform.position;
            playerDirection.y = 0f; // Y �� ������ �����Ͽ� ��� ���� ���⸸ ����մϴ�.
            Quaternion targetRotation = Quaternion.LookRotation(playerDirection);
            monster.transform.rotation = targetRotation;

        }
        else
        {
            entity.ChangeState(new MonsterIdleState());
        }
    }
}
