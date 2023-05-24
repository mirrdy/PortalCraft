using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackState : EntityState
{
    private bool canAttack = true;
    private WaitForSeconds waitForSeconds;
<<<<<<< HEAD
=======
    private MonoBehaviour monoBehaviour;
>>>>>>> KY
    private IEnumerator attack_co;
    private MonsterControl monster;
    public override void EnterState(LivingEntity entity)
    {
<<<<<<< HEAD
=======
        if (monoBehaviour == null)
        {
            monoBehaviour = entity.GetComponent<MonoBehaviour>();
        }   
        
>>>>>>> KY
        if (monster == null)
        {
            entity.TryGetComponent(out monster);
        }
        waitForSeconds = new WaitForSeconds(2f);
<<<<<<< HEAD
=======
       
       
>>>>>>> KY
    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isAttack", false);
<<<<<<< HEAD
        entity.StopCoroutine(attack_co);
=======
        monoBehaviour.StopCoroutine(attack_co);
>>>>>>> KY
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
<<<<<<< HEAD
            entity.StartCoroutine(attack_co);
=======
            monoBehaviour.StartCoroutine(attack_co);
>>>>>>> KY
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
