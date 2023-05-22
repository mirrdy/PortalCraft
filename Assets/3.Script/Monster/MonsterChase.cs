using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChase : MonoBehaviour
{
    MonsterControl monsterControl;
    private void Start()
    {
        monsterControl = transform.parent.GetComponent<MonsterControl>();
        Debug.Log("�����");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("�ε���");
            monsterControl.target = other.transform;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("������");
            monsterControl.target = other.transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("����");
        monsterControl.target = null;
    }

}
