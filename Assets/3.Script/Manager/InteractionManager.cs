using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public float interactionDistance = 5f; // 상호작용 거리
    public KeyCode interactionKey = KeyCode.F; // 상호작용 키

    private void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        // 플레이어의 조준점에서 Raycast를 사용하여 NPC와의 충돌을 확인
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
