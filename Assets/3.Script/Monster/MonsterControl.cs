using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterControl : LivingEntity
{
    public Transform target;
    public Vector3 spawnPoint;
    public float patrolRange;

    protected override void Start()
    {
        base.Start();
        Animator monsterAnimator = GetComponent<Animator>();
        animator = monsterAnimator;
        onDeath.AddListener(ItemDrop);
        spawnPoint = transform.position;
        patrolRange = 30f;

    }
    protected override void Update()
    {
        base.Update();
        Debug.Log(currentState);
        if (target != null)
        {

            Vector3 targetPosition = target.transform.position;
            targetPosition.y = transform.position.y;
            Vector3 direction = targetPosition - transform.position;
            direction.Normalize();
            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance < 3)
            {
                ChangeState(new MonsterAttackState());
            }
            else
            {
                if (!(currentState is MonsterChaseState))
                {
                    ChangeState(new MonsterChaseState());
                }
            }
        }
        if (target == null && !(currentState is MonsterPatrolState))
        {
            ChangeState(new MonsterPatrolState());
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeState(new MonsterAttackState());
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeState(new MonsterHitState());
        }
    }

    public override void OnDamage(float damage, Vector3 on, Vector3 hitNomal)
    {
        base.OnDamage(damage, on, hitNomal);
        ChangeState(new MonsterHitState());
    }
    private void ItemDrop()
    {
       
    }   
    public void DataSetting(MonsterData data)
    {
        hp = data.hp;
        atk = data.atk;
        def = data.def;
        attackTime = data.attackTime;
        moveSpeed = data.moveSpeed;
        attackRange = data.attackRange;
        patrolRange = data.patrolRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("ÃÄ¸Â¾Ñ³×");
        }
    }
}
