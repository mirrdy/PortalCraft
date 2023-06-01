using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChaseState : BossState
{
    public override void EnterState(BossControl boss)
    {
        boss.animator.SetBool("isMove", true);
    }

    public override void ExitState(BossControl boss)
    {
        boss.animator.SetBool("isMove", false);
    }

    public override void UpdateState(BossControl boss)
    {
        Vector3 targetPosition = boss.target.transform.position;
        targetPosition.y = boss.transform.position.y;
        Vector3 direction = targetPosition - boss.transform.position;
        direction.Normalize();
        float distance = Vector3.Distance(boss.transform.position, targetPosition);
        //entity.transform.position += direction * 10 * Time.deltaTime;
        boss.bossControl.Move(direction * boss.moveSpeed * Time.deltaTime);
        // ���Ͱ� �÷��̾� ���� �ٶ󺸵��� ȸ�� ����
        Vector3 playerDirection = boss.target.position - boss.transform.position;
        playerDirection.y = 0f; // Y �� ������ �����Ͽ� ��� ���� ���⸸ �����մϴ�.
        Quaternion targetRotation = Quaternion.LookRotation(playerDirection);
        boss.transform.rotation = targetRotation;

        if (distance >= 20&&boss.canRush)
        {
            boss.ChangeState(new BossRushState());
        }
        if (distance <= boss.attackRange)
        {
            int randAtk = Random.Range(1, 5);
            switch (randAtk)
            {
                case 1: boss.ChangeState(new BossWingSlashState()); break;
                case 2: boss.ChangeState(new BossWingStabAttack()); break;
                case 3: boss.ChangeState(new BossWingSpinAttack()); break;
                case 4: boss.ChangeState(new BossPullAttackState()); break;
            }       
        }
    }
}