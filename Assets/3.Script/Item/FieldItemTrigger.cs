using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItemTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerControl player))
        {
            if(transform.parent.TryGetComponent(out FieldItem item))
            {
                InGameUIManager invenUI = FindObjectOfType<InGameUIManager>();
                invenUI.AddItem(item.tagNum, item.type, item.quantity, -1); // -1 => �ʵ�������� �κ��� �� �� �Ķ����
            }
        }
    }
}
