using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChangePhase : BossState
{
    public override void EnterState(BossControl boss)
    {
        boss.phase = 2;
        boss.attackDelay = 3;
        boss.bossControl.enabled = false;
        boss.StartCoroutine(ChangePhase_co(boss));
        boss.bossMonsterSpawner.gameObject.SetActive(true);
        AudioManager.instance.PlaySFX("BossChangePhase");
    }

    public override void ExitState(BossControl boss)
    {
        boss.bossUseEffect[0].Stop();
        boss.bossUseEffect[1].Play();
        boss.bossControl.enabled = true;
        boss.bossMonsterSpawner.GetComponent<MonsterSpawner>().enabled = false;
    }

    public override void UpdateState(BossControl boss)
    {
    }

    private IEnumerator ChangePhase_co(BossControl boss)
    {
        boss.bossUseEffect[0].Play();
        yield return new WaitForSeconds(5f);
        boss.ChangeState(new BossIdleState());
    }
}
