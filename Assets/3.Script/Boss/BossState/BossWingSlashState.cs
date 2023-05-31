using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWingSlashState : BossState
{

    public override void EnterState(BossControl boss)
    {
        boss.animator.SetBool("isWingSlash", true);
        boss.atk *= boss.slashCoefficient;
    }

    public override void ExitState(BossControl boss)
    {
        boss.animator.SetBool("isWingSlash", false);
        boss.atk /= boss.slashCoefficient;
    }

    public override void UpdateState(BossControl boss)
    {   

    }
}
