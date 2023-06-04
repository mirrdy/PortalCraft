using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRushState : BossState
{
    private int coolDown =15;
    public override void EnterState(BossControl boss)
    {
        boss.animator.SetBool("isRsuh", true);
        boss.moveSpeed *= 3;
        boss.atk *= boss.rushCoefficient;
        //boss.StartCoroutine(RushCoolDown_co(boss));
    }

    public override void ExitState(BossControl boss)
    {
        boss.animator.SetBool("isRsuh", false);
        boss.moveSpeed %= 3;
        boss.atk /= boss.rushCoefficient; ;

    }

    public override void UpdateState(BossControl boss)
    {
        Vector3 targetPosition = boss.target.transform.position;
        targetPosition.y = boss.transform.position.y;
        Vector3 direction = targetPosition - boss.transform.position;
        direction.Normalize();
        //entity.transform.position += direction * 10 * Time.deltaTime;
        boss.bossControl.Move(direction * boss.moveSpeed * Time.deltaTime);
        // 몬스터가 플레이어 쪽을 바라보도록 회전 설정
        Vector3 playerDirection = boss.target.position - boss.transform.position;
        playerDirection.y = 0f; // Y 축 방향을 무시하여 평면 상의 방향만 고려합니다.
        Quaternion targetRotation = Quaternion.LookRotation(playerDirection);
        boss.transform.rotation = targetRotation;
        float distance = Vector3.Distance(boss.transform.position, targetPosition);

        if (distance <= 5)
        {
            boss.ChangeState(new BossChaseState ());
        }
    }

    //private IEnumerator RushCoolDown_co(BossControl boss)
    //{
    //    boss.canRush = false;
    //    yield return new WaitForSeconds(boss.rush.length);
    //    boss.ChangeState(new BossChaseState());
    //    yield return new WaitForSeconds(coolDown);
    //    boss.canRush = true;
    //}
}
