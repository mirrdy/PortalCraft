using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHitState : EntityState
{
    private float hitTIme = 0.5f;
    private MonoBehaviour monoBehaviour;
    public override void EnterState(LivingEntity entity)
    {
        if (monoBehaviour == null)
        {
            monoBehaviour = entity.GetComponent<MonoBehaviour>();
        }
        entity.animator.SetBool("isHit", true);
        monoBehaviour.StartCoroutine(Hit_co(entity));

    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isHit", false);
        monoBehaviour.StopCoroutine(Hit_co(entity));
    }

    public override void UpdateState(LivingEntity entity)
    {
    }

    private IEnumerator Hit_co(LivingEntity entity)
    {   
        yield return new WaitForSeconds(hitTIme);
        if (!entity.isDead)
        {
            entity.ChangeState(new MonsterIdleState());
        }

    }


}
