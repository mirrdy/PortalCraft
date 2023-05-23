using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackState : EntityState
{
    private bool canAttack = true;
    private WaitForSeconds waitForSeconds;
    private MonoBehaviour monoBehaviour;
    private IEnumerator attack_co;
    private MonsterControl monster;
    public override void EnterState(LivingEntity entity)
    {
        if (monoBehaviour == null)
        {
            monoBehaviour = entity.GetComponent<MonoBehaviour>();
        }   
        
        if (monster == null)
        {
            entity.TryGetComponent(out monster);
        }
        waitForSeconds = new WaitForSeconds(2f);
       
       
    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isAttack", false);
        monoBehaviour.StopCoroutine(attack_co);
        entity.GetComponent<CapsuleCollider>().enabled= false;
    }

    public override void UpdateState(LivingEntity entity)
    {
        if (monster.target == null)
        {
            entity.ChangeState(new MonsterReturnState());
        }
       
        Vector3 direction = monster.target.position - monster.transform.position;
        direction.Normalize();
        float distance = Vector3.Distance(monster.transform.position, monster.target.position);

        //�������� ���� ĳ�������༭ ���� ���� �ڷ�ƾ�� ������ ��ġ�� �ʵ��� ��
        attack_co = Attack_co(entity);
        if (canAttack)
        {
            canAttack = false;
            monoBehaviour.StartCoroutine(attack_co);
        }   
        if (distance > monster.attackRange)// monster attackRange�������
        {
            entity.ChangeState(new MonsterChaseState());
        }
        
        //EnterState(entity);
    }   
    private IEnumerator Attack_co(LivingEntity entity)
    {
        entity.animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(2f);
        entity.animator.SetBool("isAttack", false);
        yield return waitForSeconds;
        canAttack = true;
    }
}
