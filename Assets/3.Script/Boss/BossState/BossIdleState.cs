using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdleState : BossState
{
    public override void EnterState(BossControl boss)
    {
    }

    public override void ExitState(BossControl boss)
    {
    }

    public override void UpdateState(BossControl boss)
    {
        Vector3 targetPosition = boss.target.transform.position;
        targetPosition.y = boss.transform.position.y;
        Vector3 direction = targetPosition - boss.transform.position;
        direction.Normalize();
        float distance = Vector3.Distance(boss.transform.position, targetPosition);
        Vector3 playerDirection = boss.target.position - boss.transform.position;
        playerDirection.y = 0f; // Y 축 방향을 무시하여 평면 상의 방향만 고려합니다.
        Quaternion targetRotation = Quaternion.LookRotation(playerDirection);
        boss.transform.rotation = targetRotation;

        if (boss.target != null)
        {
            if (distance <= 5 && boss.canAttack)
            {
                boss.ChangeState(new BossChaseState());
            }
            else if (distance > 5)
            {
                boss.ChangeState(new BossChaseState());
            }
        }

    }
}
