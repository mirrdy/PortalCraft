using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterControl : LivingEntity
{
    public Transform target;
    public Vector3 spawnPoint;
    public float patrolRange;
    [SerializeField] private MonsterData monsterdata;
    [SerializeField] public float timebetAttack = 0.5f;
    public float lastAttackTimebet;
    [SerializeField]private ParticleSystem hitParticle;

    public bool isHit;
    
    private void OnEnable()
    {
        isHit = false;
        currentHp = hp; 
        isDead = false;
        entityController.enabled = true;
        currentState = new MonsterIdleState();
        ChangeState(new MonsterIdleState());
    }
    private void Awake()
    {
        entityController = GetComponent<CharacterController>();
        
    } 
    protected override void Start()
    {
        DataSetting(monsterdata);//나중에지워야함 필요없음
        base.Start();
        Animator monsterAnimator = GetComponent<Animator>();
        animator = monsterAnimator;
        onDeath.AddListener(DropItem);
        spawnPoint = transform.position;
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

    //public override void OnDamage(int damage, Vector3 on, Vector3 hitNomal)
    //{
    //    base.OnDamage(damage, on, hitNomal);
    //    if (currentHp <= 0&&!isDead)
    //    {
    //        isDead = true;
    //        ChangeState(new MonsterDieState());
    //        entityController.enabled = false;
    //    }
    //    else
    //    {
    //        ChangeState(new MonsterHitState());
    //        target = gameObject.transform;
    //    }

    //}
    public override void TakeDamage(int damage)
    {
        hitParticle.Play();
        base.TakeDamage(damage);
        AudioManager.instance.PlaySFX("HitMonster");
        if (currentHp <= 0 && !isDead)
        {
            isDead = true;
            ChangeState(new MonsterDieState());
            entityController.enabled = false;
        }
        else
        {
            isHit = true;
            ChangeState(new MonsterHitState());
            target = PlayerControl.instance.transform;
        }
    }
    //private void ItemDrop()
    //{
    //    Debug.Log("이건살행됨");
    //    for(int i = 0; i < monsterdata.dropExpNum; i++)
    //    {
    //        Instantiate(exp,transform.position, Quaternion.identity);
    //    }
    //}   
    public override void DropItem()
    {
        base.DropItem();
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
        dropExpNum = data.dropExpNum;
        name = data.name;
    }

    public void EndAttack()
    {
        if (target != null)
        {
            Vector3 direction = target.position - transform.position;
            direction.Normalize();
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance > attackRange && !(currentState is MonsterDieState))
            {
                ChangeState(new MonsterChaseState());
            }
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
