using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance = null;

    [SerializeField] private float interactionDistance = 50f; // ��ȣ�ۿ� �Ÿ�
    [SerializeField] private KeyCode interactionKey = KeyCode.F; // ��ȣ�ۿ� Ű
    private GameObject interactionUI; // ��ȣ�ۿ� Ű ǥ�� UI
    public bool isInteracting;

    // �׽�Ʈ�� �ӽ� ������Ʈ, ���� ����
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
        // �÷��̾��� ���������� Raycast�� ����Ͽ� NPC���� �浹�� Ȯ��
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
                    // ��ȣ�ۿ� Ű �˷��ִ� UI Ȱ��ȭ
                    interactionUI.SetActive(true);
                }
                // ��ȣ�ۿ� Ű �������� Ȯ��
                if (Input.GetKeyDown(interactionKey))
                {
                    interactiveEntity.Interact();
                }
            }
            else
            {
                if (interactionUI.activeSelf)
                {
                    // NPC���� �浹�� ���� ��� UI ��Ȱ��ȭ
                    interactionUI.SetActive(false);
                }
            }
        }


        /*// �ӽ� 
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
