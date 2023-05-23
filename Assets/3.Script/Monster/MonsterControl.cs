using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterControl : LivingEntity
{
    public Transform target;
    public Vector3 spawnPoint;
    public float patrolRange;
    [SerializeField] private MonsterData monsterdata;

    protected override void Start()
    {
        base.Start();
        Animator monsterAnimator = GetComponent<Animator>();
        animator = monsterAnimator;
        onDeath.AddListener(ItemDrop);
        spawnPoint = transform.position;
        DataSetting(monsterdata);//나중에지워야함 필요없음

    }
    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeState(new MonsterHitState());
        }
    }

    public override void OnDamage(float damage, Vector3 on, Vector3 hitNomal)
    {
        base.OnDamage(damage, on, hitNomal);
        ChangeState(new MonsterHitState());
        target = gameObject.transform;
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
            Debug.Log("쳐맞앗네");
            CapsuleCollider attackCol = GetComponent<CapsuleCollider>();
            attackCol.enabled = false;
        }
    }
}
