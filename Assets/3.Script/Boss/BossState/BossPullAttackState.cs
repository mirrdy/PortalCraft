using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPullAttackState : BossState
{
    public override void EnterState(BossControl boss)
    {
        boss.animator.SetBool("isPullAttack", true);
        boss.atk *= boss.pullCoefficient;
    }

    public override void ExitState(BossControl boss)
    {
        boss.animator.SetBool("isPullAttack", false);
        boss.atk /= boss.pullCoefficient;
    }

    public override void UpdateState(BossControl boss)
    {
    }
}
