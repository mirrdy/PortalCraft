using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterControl : LivingEntity
{
    private GameObject[] dropItemList;
    public Transform target;

    protected override void Start()
    {
        base.Start();
        Animator monsterAnimator = GetComponent<Animator>();
        animator = monsterAnimator;
        onDeath.AddListener(ItemDrop);

    }
    protected override void Update()
    {
        base.Update();
        Debug.Log(currentState);
        if (target != null)
        {
            Vector3 direction = target.transform.position - transform.position;
            float distance = direction.magnitude;
            if (distance <= 2)
            {
                ChangeState(new MonsterAttackState());
            }
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
       for(int i =0; i < dropItemList.Length; i++)
        {   
            Instantiate(dropItemList[i], transform.position, Quaternion.identity);
        }
    }   
    public void DataSetting(MonsterData data)
    {
        hp = data.hp;
        atk = data.atk;
        def = data.def;
        attackTime = data.attackTime;
        dropItemList = data.dropItemList;
        moveSpeed = data.moveSpeed;
        attackRange = data.attackRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
        }
    }
}
