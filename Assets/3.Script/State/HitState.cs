using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : EntityState
{
    private float hitTIme = 0.5f;
    private MonoBehaviour monoBehaviour;
    public override void EnterState(LivingEntity entity)
    {
        entity.TryGetComponent(out this.monoBehaviour);
        entity.animator.SetBool("isHit", true);
        monoBehaviour.StartCoroutine(Hit_co(entity));

    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isHit", false);
    }

    public override void UpdateState(LivingEntity entity)
    {
    }

    private IEnumerator Hit_co(LivingEntity entity)
    {
        yield return new WaitForSeconds(hitTIme);
        entity.ChangeState(new IdleState());

    }


}
