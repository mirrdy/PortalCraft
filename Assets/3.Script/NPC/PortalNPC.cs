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
    public PortalController portal;
    public int portalIndex;

    private void Start()
    {
        GameObject.Find("InGameUIManager").TryGetComponent(out ingameUIManager);
    }

    public override void Interact()
    {
        ShowDialog();

        Debug.Log($"{gameObject.name}: 상호작용 성공!");
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
            TMP.text = $"T{portalIndex+1} 포탈조각을 10개 가져오면 포탈을 활성화시켜줄게.";
            progress = QuestProgress.Doing;
        }
        else if(progress == QuestProgress.Doing)
        {
            // 인벤토리에서 재료가 있는지 없는지 확인 필요
            if ((portalIndex == 0 && ingameUIManager.InventoryItemCheck(401, 10)) ||
                (portalIndex == 1 && ingameUIManager.InventoryItemCheck(402, 10)))
            {
                TMP.text = "가져온 조각으로 포탈을 활성화시켰어.";
                portal.enabled = true;
                progress = QuestProgress.Finish;
            }
            else
            {
                TMP.text = $"아직 T{portalIndex + 1} 포탈조각 10개를 모으지 못했는데?";
            }
        }
        else if(progress == QuestProgress.Finish)
        {
            TMP.text = "포탈을 활성화했어. 다음 섬으로 이동해봐.";
        }
        StartCoroutine(ActiveOffDialog(dialogFrame));
    }
    IEnumerator ActiveOffDialog(GameObject dialogFrame)
    {
        yield return new WaitForSeconds(5f);
        dialogFrame.gameObject.SetActive(false);
    }
}
