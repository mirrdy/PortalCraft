using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChase : MonoBehaviour
{
    MonsterControl monsterControl;
    private void Start()
    {
        monsterControl = transform.parent.GetComponent<MonsterControl>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            monsterControl.target = other.transform;
            monsterControl.ChangeState(new MonsterChaseState());
        }
    }
    private void OnTriggerStay(Collider other)
    {
        monsterControl.target = other.transform;
    }
    private void OnTriggerExit(Collider other)
    {
        monsterControl.target = null;
        monsterControl.ChangeState(new MonsterReturnState());
    }

}
