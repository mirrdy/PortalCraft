using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance = null;

    [SerializeField] private float interactionDistance = 1f; // 상호작용 거리
    [SerializeField] private KeyCode interactionKey = KeyCode.F; // 상호작용 키
    [SerializeField] private GameObject interactionUIInfo; // 상호작용 키 표시 UI
    private GameObject interactionUI;
    public bool isInteracting;

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

        interactionUI = Instantiate(interactionUIInfo);
        interactionUI.SetActive(false);
    }
    private void Update()
    {
        if(isInteracting)
        {
            if(Input.GetKeyDown(interactionKey))
            {
                if (GameObject.Find("InGameUIManager").TryGetComponent(out InGameUIManager ui))
                {
                    ui.OffCraftTable();
                    isInteracting = false;
                    return;
                }
            }
        }
        // 플레이어랑 가까이 있으면 상호작용 가능
        Collider[] cols = Physics.OverlapSphere(PlayerControl.instance.transform.position, interactionDistance);
        foreach(Collider col in cols)
        {
            if(col.TryGetComponent(out InteractiveEntity interactiveEntity))
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
                return;
            }
        }
        if(interactionUI.activeSelf)
        {
            // NPC와의 충돌이 없는 경우 UI 비활성화
            interactionUI.SetActive(false);
        }

       /* if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
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
        }*/
    }
}
