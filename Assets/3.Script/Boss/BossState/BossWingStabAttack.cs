using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWingStabAttack : BossState
{
    public override void EnterState(BossControl boss)
    {
        boss.animator.SetBool("isWingStab", true);
        boss.atk *= boss.stabCoefficient;
    }

    public override void ExitState(BossControl boss)
    {
        boss.animator.SetBool("isWingStab", false);
        boss.atk /= boss.stabCoefficient;
    }

    public override void UpdateState(BossControl boss)
    {   

    }
}
