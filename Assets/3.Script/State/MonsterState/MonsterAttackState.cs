using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackState : EntityState
{
    //private bool canAttack = true;
    //private WaitForSeconds waitForSeconds;
    //private IEnumerator attack_co;
    private MonsterControl monster;
    public override void EnterState(LivingEntity entity)
    {
        if (monster == null)
        {
            entity.TryGetComponent(out monster);
        }
        monster.animator.SetBool("isAttack", true);
        //waitForSeconds = new WaitForSeconds(2f);
    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isAttack", false);
        entity.StartCoroutine(AttackCool_co(monster));
        //entity.StopCoroutine(attack_co);
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
        //attack_co = Attack_co(entity);
        //if (canAttack)
        //{
        //    canAttack = false;
        //    entity.StartCoroutine(attack_co);
        //}   
        if (distance > monster.attackRange)// monster attackRange�������
        {
            entity.ChangeState(new MonsterChaseState());
        }
        
        //EnterState(entity);
    }   
    //private IEnumerator Attack_co(LivingEntity entity)
    //{
    //    entity.animator.SetBool("isAttack", true);
    //    yield return new WaitForSeconds(2f);
    //    entity.animator.SetBool("isAttack", false);
    //    yield return waitForSeconds;
    //    canAttack = true;
    //}
    private IEnumerator AttackCool_co(MonsterControl monster)
    {
        monster.canAttack = false;
        yield return new WaitForSeconds(monster.attackTime);
        monster.canAttack = true;
    }
}
