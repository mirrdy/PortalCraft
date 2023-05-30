using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossControl : MonoBehaviour
{
    public float hp { get; private set; }
    public float def { get; private set; }
    public float atk { get; private set; }
    public float attackTime { get; private set; }
    public float moveSpeed { get; private set; }
    public float attackRange { get; private set; }
    public bool isDead { get; private set; }

    public BossState currentState;

    [SerializeField]private MonsterData bossData;
    private void Start()
    {
        DataSetting(bossData);
        currentState = new BossIdleState();
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
        hp -= (damage - def);
    }



}
