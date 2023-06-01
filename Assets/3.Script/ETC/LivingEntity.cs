using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour, IDamage
{
    public UnityEvent onDeath;
    public float hp { get; protected set; }

    public float currentHp;
    public float def { get; protected set; }
    public float atk { get; protected set; }
    public float attackTime { get; protected set; }
    public float moveSpeed { get; protected set; }
    public float attackRange { get; protected set; }
    public bool isDead { get; protected set; }
    public float gravity { get; protected set; }     // 중력 계수
    public Animator animator;
    public CharacterController entityController;

    protected EntityState currentState;
   
    protected virtual void Start()
    {
        // 초기 상태로 IdleState를 설정
        currentHp = hp;
        currentState = new MonsterIdleState();
        ChangeState(new MonsterIdleState());
    }
    protected virtual void Update()
    {
        currentState.UpdateState(this);
        if (!entityController.isGrounded)
        {
            entityController.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
        }
    }
    public void ChangeState(EntityState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    public virtual void OnDamage(int damage, Vector3 on, Vector3 hitNomal)
    {
        hp -= damage - (damage * Mathf.RoundToInt(def / (def + 50) * 100) * 0.01f);
    }



}
