using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/monsterData", fileName = "MonsterData")]
public class MonsterData : ScriptableObject
{
    public float hp = 50f;
    public float atk = 50f;
    public float def = 50f;
    public float attackTime = 1f;
    public float moveSpeed = 0.5f;
    public float attackRange = 2f;
    public float patrolRange = 2f;
    public float gravity = -6f;
    public int dropExpNum = 2;
    


}
