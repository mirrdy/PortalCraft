using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PortalNPC : InteractiveEntity
{
    private enum QuestProgress
    {
        NoStart,
        Doing,
        Finish
    }
    private QuestProgress progress;
    private GameObject canvas_Dialog;
    private InGameUIManager ingameUIManager;

    private void Start()
    {
        GameObject.Find("InGameUIManager").TryGetComponent(out ingameUIManager);
    }

    public override void Interact()
    {
        ShowDialog();

        Debug.Log($"{gameObject.name}: ��ȣ�ۿ� ����!");
        InteractionManager.instance.isInteracting = false;
    }
    private void ShowDialog()
    {
        canvas_Dialog = GameObject.Find("DialogCanvas");
        GameObject dialogFrame = canvas_Dialog.transform.Find("DialogFrame").gameObject;
        dialogFrame.gameObject.SetActive(true);
        dialogFrame.transform.Find("DialogText").TryGetComponent(out TextMeshProUGUI TMP);

        if(progress == QuestProgress.NoStart)
        {
            TMP.text = "��Ż������ 10�� �������� ��Ż�� Ȱ��ȭ�����ٰ�.";
            progress = QuestProgress.Doing;
        }
        else if(progress == QuestProgress.Doing)
        {
            // �κ��丮���� ��ᰡ �ִ��� ������ Ȯ�� �ʿ�
            
            TMP.text = "������ �������� ��Ż�� Ȱ��ȭ�������.";
        }
        else if(progress == QuestProgress.Finish)
        {
            TMP.text = "��Ż�� Ȱ��ȭ�Ǿ��־�. ��Ż�� �̵��غ�.";
        }
        StartCoroutine(ActiveOffDialog(dialogFrame));
    }
    IEnumerator ActiveOffDialog(GameObject dialogFrame)
    {
        yield return new WaitForSeconds(5f);
        dialogFrame.gameObject.SetActive(false);
    }
    private void UpdateQuestProgress()
    {

    }
}
