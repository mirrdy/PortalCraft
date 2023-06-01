using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDieState : EntityState
{
    public override void EnterState(LivingEntity entity)
    {
        if (entity.onDeath != null && entity.isDead)
        {
            entity.onDeath.Invoke();
            entity.StartCoroutine(Die_co(entity));

        }
    }

    public override void ExitState(LivingEntity entity)
    {
        
    }

    public override void UpdateState(LivingEntity entity)
    {
            
    }
    private IEnumerator Die_co(LivingEntity entity)
    {
        Debug.Log("�� �ڷ�ƾ ��� ���۵�");
        entity.animator.SetBool("isDead", true);
        yield return new WaitForSeconds(5f);
        entity.animator.SetBool("isDead", false);
        Debug.Log("��������");
        entity.gameObject.SetActive(false);
        entity.ChangeState(new MonsterIdleState());
    }
}
