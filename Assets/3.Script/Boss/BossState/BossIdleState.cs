using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdleState : BossState
{
    public override void EnterState(BossControl boss)
    {
    }

    public override void ExitState(BossControl boss)
    {
    }

    public override void UpdateState(BossControl boss)
    {
        if (boss.target != null)
        {
            boss.ChangeState(new BossSpawnState());
        }
    }
}
