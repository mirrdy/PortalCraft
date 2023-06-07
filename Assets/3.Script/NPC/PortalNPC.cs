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
            TMP.text = "포탈조각을 10개 가져오면 포탈을 활성화시켜줄게.";
            progress = QuestProgress.Doing;
        }
        else if(progress == QuestProgress.Doing)
        {
            // 인벤토리에서 재료가 있는지 없는지 확인 필요
            
            TMP.text = "가져온 조각으로 포탈을 활성화시켜줬어.";
        }
        else if(progress == QuestProgress.Finish)
        {
            TMP.text = "포탈은 활성화되어있어. 포탈로 이동해봐.";
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
