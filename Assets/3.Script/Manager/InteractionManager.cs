using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance = null;

    [SerializeField] private float interactionDistance = 1f; // ��ȣ�ۿ� �Ÿ�
    [SerializeField] private KeyCode interactionKey = KeyCode.F; // ��ȣ�ۿ� Ű
    [SerializeField] private GameObject interactionUIInfo; // ��ȣ�ۿ� Ű ǥ�� UI
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
        // �÷��̾�� ������ ������ ��ȣ�ۿ� ����
        Collider[] cols = Physics.OverlapSphere(PlayerControl.instance.transform.position, interactionDistance);
        foreach(Collider col in cols)
        {
            if(col.TryGetComponent(out InteractiveEntity interactiveEntity))
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
                return;
            }
        }
        if(interactionUI.activeSelf)
        {
            // NPC���� �浹�� ���� ��� UI ��Ȱ��ȭ
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
        }*/
    }
}
