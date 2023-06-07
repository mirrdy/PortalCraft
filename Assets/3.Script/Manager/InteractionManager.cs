using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance = null;

    [SerializeField] private float interactionDistance = 50f; // 상호작용 거리
    [SerializeField] private KeyCode interactionKey = KeyCode.F; // 상호작용 키
    private GameObject interactionUI; // 상호작용 키 표시 UI
    public bool isInteracting;

    // 테스트용 임시 오브젝트, 삭제 예정
    [SerializeField] private GameObject NPC;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        interactionUI = GetComponentInChildren<TMPro.TextMeshPro>().gameObject;
    }
    private void Update()
    {
        // 플레이어의 조준점에서 Raycast를 사용하여 NPC와의 충돌을 확인
        Vector2 screenCenterPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPos);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            Debug.Log($"{hit.transform.name}");
            if (hit.collider.TryGetComponent(out InteractiveEntity interactiveEntity))
            {
                interactionUI.transform.position = interactiveEntity.transform.position + new Vector3(0.5f, 1.5f, 0);
                if (!interactionUI.activeSelf)
                {
                    // 상호작용 키 알려주는 UI 활성화
                    interactionUI.SetActive(true);
                }
                // 상호작용 키 눌렀는지 확인
                if (Input.GetKeyDown(interactionKey))
                {
                    interactiveEntity.Interact();
                }
            }
            else
            {
                if (interactionUI.activeSelf)
                {
                    // NPC와의 충돌이 없는 경우 UI 비활성화
                    interactionUI.SetActive(false);
                }
            }
        }


        /*// 임시 
        if (!isInteracting)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                NPC = FindObjectOfType<PortalNPC>().gameObject;
                if (NPC.TryGetComponent(out InteractiveEntity interactiveEntity))
                {
                    interactiveEntity.Interact();
                }
            }
        }*/
    }
}
