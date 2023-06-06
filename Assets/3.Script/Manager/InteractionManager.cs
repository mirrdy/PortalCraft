using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public float interactionDistance = 5f; // ��ȣ�ۿ� �Ÿ�
    public KeyCode interactionKey = KeyCode.F; // ��ȣ�ۿ� Ű

    private void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        // �÷��̾��� ���������� Raycast�� ����Ͽ� NPC���� �浹�� Ȯ��
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if(hit.collider.TryGetComponent(out InteractiveEntity interactiveEntity))
            {
                interactiveEntity.Interact();
            }
        }
    }
}
