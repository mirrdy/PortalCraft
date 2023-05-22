using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChase : MonoBehaviour
{
    MonsterControl monsterControl;
    private void Start()
    {
        monsterControl = transform.parent.GetComponent<MonsterControl>();
        Debug.Log("½ÇÇàµÊ");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("ºÎµúÈû");
            monsterControl.target = other.transform;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("À¯ÁöÁß");
            monsterControl.target = other.transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("³ª°¨");
        monsterControl.target = null;
    }

}
