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
        if (distance >= boss.attackRange)
        {
            //entity.transform.position += direction * 10 * Time.deltaTime;
            boss.bossControl.Move(direction * boss.moveSpeed * Time.deltaTime);
            // 몬스터가 플레이어 쪽을 바라보도록 회전 설정
            Vector3 playerDirection = boss.target.position - boss.transform.position;
            playerDirection.y = 0f; // Y 축 방향을 무시하여 평면 상의 방향만 고려합니다.
            Quaternion targetRotation = Quaternion.LookRotation(playerDirection);
            boss.transform.rotation = targetRotation;
           
        }
        if(distance<= boss.attackRange * 4 && boss.canAttack)
        {
            if (boss.phase == 1)
            {
                boss.ChangeState(new BossCastState());
            }
            else
            {
                int randNum = Random.Range(0, 2);
                switch (randNum)
                {
                    case 0: boss.ChangeState(new BossCastState()); break;
                    case 1: boss.ChangeState(new BossWingSlashState()); break;  
                }
            }
        }
        if(distance<= boss.attackRange * 3 && distance > boss.attackRange && boss.canAttack&&boss.phase==1)
        {
            boss.ChangeState(new BossWingSlashState());
        }
        if (distance <= boss.attackRange && boss.canAttack)
        {
            int randAtk = Random.Range(1, 6);
            switch (randAtk)
            {
                case 1: boss.ChangeState(new BossWingSlashState()); break;
                case 2: boss.ChangeState(new BossWingStabAttack()); break;
                case 3: boss.ChangeState(new BossWingSpinAttack()); break;
                case 4: boss.ChangeState(new BossPullAttackState()); break;
                case 5: boss.ChangeState(new BossCastState()); break;
            }
        }
        if (distance <= boss.attackRange && !boss.canAttack)
        {
            boss.ChangeState(new BossIdleState());
        }
    }
}
