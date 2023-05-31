using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossControl : MonoBehaviour
{
    public float hp;
    public float def;
    public float atk;
    public float attackTime;
    public float moveSpeed;
    public float attackRange;
    public bool isDead;
    //데미지계수
    public float rushCoefficient = 0.7f;
    public float slashCoefficient = 1f;
    public float stabCoefficient = 1.2f;
    public float spinCoefficient = 1.4f;
    public float pullCoefficient = 1.5f;
    public float magicCoefficient = 1f;

    public BossState currentState;

    public Animator animator;

    public bool canRush;

    public Transform target;

    public CharacterController bossControl;
    [SerializeField]private MonsterData bossData;
    private void Start()
    {
        animator = GetComponent<Animator>();
        DataSetting(bossData);
        currentState = new BossIdleState();
        bossControl = GetComponent<CharacterController>();
            
    }
    private void Update()   
    {
        currentState.UpdateState(this);
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

    public virtual void OnDamage(float damage, Vector3 on, Vector3 hitNomal)
    {
        hp -= damage-(damage * Mathf.RoundToInt(def/(def+50)*100)*0.01f);
    }
    public void EndAttack()
    {
        currentState.ExitState(this);
        currentState = new BossChaseState();
        currentState.EnterState(this);
    }



}
