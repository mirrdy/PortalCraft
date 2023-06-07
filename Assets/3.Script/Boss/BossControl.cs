using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossControl : MonoBehaviour ,IDestroyable
{
    public UnityEvent onDeath;

    public float hp;
    public float currentHp;
    public float def;
    public float atk;
    public float attackTime;
    public float moveSpeed;
    public float attackRange;
    public bool isDead;
    public bool canAttack;
    public int phase = 1;
    public int attackDelay = 5;

    private Vector3 bossSpawn;

    [SerializeField] public float timebetAttack = 0.5f;
    public float lastAttackTimebet;
    //���������
    public float rushCoefficient = 0.7f;
    public float slashCoefficient = 1f;
    public float stabCoefficient = 1.2f;
    public float spinCoefficient = 1.4f;
    public float pullCoefficient = 1.5f;
    public float magicCoefficient = 0.7f;



    public BossState currentState;

    public Animator animator;

    //public bool canRush;

    public Transform target;

    public CharacterController bossControl;
    [SerializeField] private MonsterData bossData;
    [SerializeField] public MonsterSpawner bossMonsterSpawner;
    //���� ��ƼŬ
    [SerializeField] private ParticleSystem hitParticle;
    [SerializeField] public ParticleSystem[] bossUseEffect;
    

    //�÷��̾� �渷 ������Ʈ
    [SerializeField] private GameObject playerBlock;

    private void Awake()
    {
        DataSetting(bossData);
        currentHp = hp;
        canAttack = true;
        animator = GetComponent<Animator>();
        currentState = new BossIdleState();
        ChangeState(new BossIdleState());
        bossControl = GetComponent<CharacterController>();
        onDeath.AddListener(DropItem);
        onDeath.AddListener(OpenDoor);
        bossSpawn = transform.position;
    }

    private void OnEnable()
    {
        playerBlock.SetActive(false);
        gameObject.transform.position = bossSpawn;
    }
    //private void Start()
    //{
    //    DataSetting(bossData);
    //    currentHp = hp;
    //    canAttack = true;
    //    animator = GetComponent<Animator>();
    //    currentState = new BossIdleState();
    //    ChangeState(new BossIdleState());
    //    bossControl = GetComponent<CharacterController>();
    //    onDeath.AddListener(DropItem);
    //    onDeath.AddListener(OpenDoor);
            
    //}
    private void Update()   
    {
        currentState.UpdateState(this);
        //if (input.getkeydown(keycode.k))
        //{
        //    changephase();
        //}
    }
    public void DataSetting(MonsterData data)
    {
        hp = data.hp;
        atk = data.atk;
        def = data.def;
        attackTime = data.attackTime;
        moveSpeed = data.moveSpeed;
        attackRange = data.attackRange;
    }

    public void ChangeState(BossState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    //public virtual void OnDamage(int damage, Vector3 on, Vector3 hitNomal)
    //{
    //    hp -= damage-(damage * Mathf.RoundToInt(def/(def+50)*100)*0.01f);
    //}
    public void EndAttack()
    {
        StartCoroutine(AttackTime_co());
    }

    public void DropItem()
    {

    }

    public void TakeDamage(int damage)
    {
        hitParticle.Play();
        damage = damage - Mathf.RoundToInt(damage * Mathf.RoundToInt(def / (def + 50) * 100) * 0.01f);
        currentHp -= damage;
        if (currentHp <= 0 && !isDead)
        {
            isDead = true;
            ChangeState(new BossDieState());
            //entityController.enabled = false;
        }
        if(currentHp <= currentHp*0.5&&phase == 1)
        {
            ChangePhase();
        }
    }
    private IEnumerator AttackTime_co()
    {
        canAttack = false;
        ChangeState(new BossIdleState());
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }
    public void ChangePhase()
    {
        ChangeState(new BossChangePhase());

    }
    private void OpenDoor()
    {
        playerBlock.SetActive(false);
    }
}
