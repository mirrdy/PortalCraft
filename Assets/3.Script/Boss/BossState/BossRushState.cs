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
