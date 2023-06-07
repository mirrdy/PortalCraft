using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWingSpinAttack : BossState
{
    public override void EnterState(BossControl boss)
    {
        boss.animator.SetBool("isSpinWingSlash", true);
        boss.atk *= boss.spinCoefficient;
        AudioManager.instance.PlaySFX("BossSpinAttack");
    }

    public override void ExitState(BossControl boss)
    {
        boss.animator.SetBool("isSpinWingSlash", false);
        boss.atk /= boss.spinCoefficient;
    }

    public override void UpdateState(BossControl boss)
    {
    }
}
