using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour
{
    public UnityEvent onDeath;
    public float hp { get; protected set; }
    public float def { get; protected set; }
    public float atk { get; protected set; }
    public float attackTime { get; protected set; }
    public float moveSpeed { get; protected set; }
    public float attackRange { get; protected set; }
    public bool isDead { get; protected set; }

    public Animator animator;

    protected EntityState currentState;
    protected virtual void Start()
    {
        // 초기 상태로 IdleState를 설정
        currentState = new MonsterIdleState();
       
    }
    protected virtual void Update()
    {
        currentState.UpdateState(this);
    }
    public void ChangeState(EntityState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    public virtual void OnDamage(float damage, Vector3 on, Vector3 hitNomal)
    {
        hp -= damage;
    }



}
