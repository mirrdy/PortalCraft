using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCastState : BossState
{
    public override void EnterState(BossControl boss)
    {
        boss.bossUseEffect[3].gameObject.SetActive(true);
        boss.animator.SetBool("isCasting", true);
        boss.bossUseEffect[3].Play();
        boss.atk *= boss.magicCoefficient;
        
    }

    public override void ExitState(BossControl boss)
    {
        boss.bossUseEffect[3].gameObject.SetActive(false);
        boss.animator.SetBool("isCasting", false);
        boss.bossUseEffect[3].Stop();
        boss.atk /= boss.magicCoefficient;


    }

    public override void UpdateState(BossControl boss)
    {
        boss.bossUseEffect[3].transform.position = boss.target.transform.position;
    }
}
