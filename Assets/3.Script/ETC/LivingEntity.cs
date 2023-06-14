using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour, IDestroyable
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

    public int dropExpNum;
    public Animator animator;
    public CharacterController entityController;

    [SerializeField] GameObject exp;

    public List<FieldItem> dropTables;

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

    //public virtual void OnDamage(int damage, Vector3 on, Vector3 hitNomal)
    //{
    //   damage = damage - Mathf.RoundToInt(damage * Mathf.RoundToInt(def / (def + 50) * 100) * 0.01f);
    //    currentHp -= damage;
    //}

    public virtual void DropItem()
    {
        for (int i = 0; i < dropExpNum; i++)
        {
            Vector3 dropPoint = transform.position+ Random.insideUnitSphere;
            dropPoint.y = transform.position.y;
            Instantiate(exp, dropPoint, Quaternion.identity);
        }
        if (dropTables.Count > 0)
        {
            int rand = Random.Range(1, 4);
            Vector3 dropPoint = transform.position + Random.insideUnitSphere;
            dropPoint.y = transform.position.y;
            Vector3 dropPointSecond = transform.position + Random.insideUnitSphere;
            dropPointSecond.y = transform.position.y+1f;
            switch (rand)
            {
                case 1:
                    Instantiate(dropTables[0], dropPoint, Quaternion.identity);
                    break;

                case 2:
                    Instantiate(dropTables[0], dropPoint, Quaternion.identity);
                    Instantiate(dropTables[1], dropPointSecond, Quaternion.identity);
                    break;
                case 3:
                    Instantiate(dropTables[1], dropPointSecond, Quaternion.identity);
                    break;
            }
        }

    }

    public virtual void TakeDamage(int damage)
    {
        damage = damage - Mathf.RoundToInt(damage * Mathf.RoundToInt(def / (def + 50) * 100) * 0.01f);
        currentHp -= damage;
    }
}
