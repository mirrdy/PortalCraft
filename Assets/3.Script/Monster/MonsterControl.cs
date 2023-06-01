using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterControl : LivingEntity
{
    public Transform target;
    public Vector3 spawnPoint;
    public float patrolRange;
    [SerializeField] private MonsterData monsterdata;
    public bool canAttack;
    [SerializeField] public float timebetAttack = 0.5f;
    public float lastAttackTimebet;

    private void OnEnable()
    {
        currentHp = hp;
        isDead = false;
        entityController.gameObject.SetActive(true);

        currentState = new MonsterIdleState();
        ChangeState(new MonsterIdleState());
    }

    protected override void Start()
    {
        DataSetting(monsterdata);//나중에지워야함 필요없음
        canAttack = true;
        base.Start();
        Animator monsterAnimator = GetComponent<Animator>();
        animator = monsterAnimator;
        onDeath.AddListener(ItemDrop);
        spawnPoint = transform.position;
        entityController = GetComponent<CharacterController>();
        target = null;
        isDead = false;

    }
    protected override void Update()
    {
        base.Update();
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    ChangeState(new MonsterHitState());
        //}
    }

    public override void OnDamage(int damage, Vector3 on, Vector3 hitNomal)
    {
        base.OnDamage(damage, on, hitNomal);
        Debug.Log(currentHp);
        if (currentHp <= 0&&!isDead)
        {
            isDead = true;
            ChangeState(new MonsterDieState());
            //entityController.gameObject.SetActive(false);
        }
        else
        {
            ChangeState(new MonsterHitState());
            target = gameObject.transform;
        }

    }
    private void ItemDrop()
    {
        Debug.Log("아이템떨굼 나는 두ㅢ짐");
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
        gravity = data.gravity;
    }

    public void EndAttack()
    {
        if (!(currentState is MonsterDieState))
        {
            ChangeState(new MonsterChaseState());
        }
    }
    private void Die()
    {
        if (onDeath != null)
        {
            onDeath.Invoke();
        }
        animator.SetBool("isDead", true);
    }
}
