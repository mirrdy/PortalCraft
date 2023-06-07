using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWingSlashState : BossState
{

    public override void EnterState(BossControl boss)
    {
        boss.animator.SetBool("isWingSlash", true);
        boss.atk *= boss.slashCoefficient;
        AudioManager.instance.PlaySFX("BossSlash");
        if (boss.phase == 2)
        {
            boss.bossUseEffect[2].Play();
            AudioManager.instance.PlaySFX("BossFreeze");
        }
    }

    public override void ExitState(BossControl boss)
    {
        boss.animator.SetBool("isWingSlash", false);
        boss.atk /= boss.slashCoefficient;
        boss.bossUseEffect[2].Stop();

    }

    public override void UpdateState(BossControl boss)
    {   

    }
}
