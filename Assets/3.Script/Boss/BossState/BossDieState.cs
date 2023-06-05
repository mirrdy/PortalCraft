using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDieState : BossState
{
    public override void EnterState(BossControl boss)
    {
         if (boss.onDeath != null && boss.isDead)
        {
            boss.onDeath.Invoke();
            boss.StartCoroutine(Die_co(boss));

        }
    }

    public override void ExitState(BossControl boss)
    {
        boss.animator.ResetTrigger("isDie");
        boss.gameObject.SetActive(false);
    }

    public override void UpdateState(BossControl boss)
    {

    }

    private IEnumerator Die_co(BossControl boss)
    {
        boss.animator.SetTrigger("isDie");
        yield return new WaitForSeconds(5f);
        boss.ChangeState(new BossIdleState());

    }


}
