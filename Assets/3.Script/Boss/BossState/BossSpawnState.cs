using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawnState : BossState
{
    public override void EnterState(BossControl boss)
    {
        boss.animator.SetBool("isSpawn", true);
        boss.animator.SetBool("isStart", true);
    }

    public override void ExitState(BossControl boss)
    {
        boss.animator.SetBool("isSpawn", false);

    }

    public override void UpdateState(BossControl boss)
    {

    }
    private IEnumerator Sapwn_co(BossControl boss)
    {
        yield return new WaitForSeconds(3f);

    }
}
