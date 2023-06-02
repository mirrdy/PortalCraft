using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNonAtk : MonoBehaviour
{
    MonsterControl monsterControl;
    private void Start()
    {
        monsterControl = transform.parent.GetComponent<MonsterControl>();
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")&&monsterControl.target!= null )
        {
            monsterControl.target = other.transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        monsterControl.target = null;
    }

}
