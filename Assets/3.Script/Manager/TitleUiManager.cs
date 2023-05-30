using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TitleUiManager : MonoBehaviour
{
    [Header("Windows")]
    [SerializeField] GameObject settingWindow;
    [SerializeField] GameObject resolutionWindow;
    [SerializeField] TMP_Text resolutionCount;

    [Header("Setting Window")]
    [SerializeField] TMP_Dropdown resolution;
    [SerializeField] Slider slider_Bgm;
    [SerializeField] Slider slider_Sfx;

    [Header("Icon Image")]
    [SerializeField] Image image_Bgm;
    [SerializeField] Image image_Sfx;
    [SerializeField] Sprite[] sprite_Bgm;
    [SerializeField] Sprite[] sprite_Sfx;

    [Header("Continu Btn")]
    [SerializeField] GameObject onContinu;
    [SerializeField] GameObject offContinu;

    [Header("NewGame Btn")]
    [SerializeField] GameObject image_Message;

    [Header("Continu Btn")]
    [SerializeField] GameObject image_DataSlot;
    [SerializeField] GameObject image_DeleteData;
    [SerializeField] Text[] playerInpomation;

    private bool isResolution = false;
    private int saveDataNumber = 0;

    private void Start()
    {
        slider_Bgm.value = DataManager.instance.LoadSound()[0];
        slider_Sfx.value = DataManager.instance.LoadSound()[1];
        resolution.value = DataManager.instance.LoadResolution();
    }

    private void OnEnable()
    {
        ContinuBtnOnOff();

        settingWindow.SetActive(false);
        resolutionWindow.SetActive(false);
        image_Message.SetActive(false);
        image_DataSlot.SetActive(false);
    }

    public void BGM_VolumeSetting()  // 배경음 소리 설정
    {
        AudioManager.instance.bgmPlay.volume = slider_Bgm.value;

        if (slider_Bgm.value == 0)
        {
            image_Bgm.sprite = sprite_Bgm[1];
        }
        else
        {
            image_Bgm.sprite = sprite_Bgm[0];
        }
    }

    public void SFX_VolumeSetting()  // 효과음 소리 설정
    {
        for (int i = 0; i < AudioManager.instance.sfxPlay.Length; i++)
        {
            AudioManager.instance.sfxPlay[i].volume = slider_Sfx.value;
        }

        if (slider_Sfx.value == 0)
        {
            image_Sfx.sprite = sprite_Sfx[1];
        }
        else
        {
            image_Sfx.sprite = sprite_Sfx[0];
        }
    }

    public void SaveButton()  // 저장 버튼 메소드
    {
        DataManager.instance.SaveSound(slider_Bgm.value, slider_Sfx.value);
    }

    public void BackButton()  // 뒤로가기 버튼 메소드
    {
        isResolution = false;

        slider_Bgm.value = DataManager.instance.LoadSound()[0];
        slider_Sfx.value = DataManager.instance.LoadSound()[1];
        settingWindow.SetActive(false);
    }

    public void CheckSound(BaseEventData eventdata)  // 슬라이더 소리 변경시 사운드 체크 메소드
    {
        AudioManager.instance.PlaySFX("SoundCheck");
    }

    public void SettingWindow()  // 옵션 창 호출 버튼 메소드
    {
        isResolution = true;
        settingWindow.SetActive(true);
    }

    public void GameQuit()  // 게임 종료 메소드
    {
        Application.Quit();
    }

    public void DropDownValue()
    {
        if (!isResolution)
        {
            return;
        }

        if (resolution.value == 0)
        {
            Screen.SetResolution(1920, 1080, false);
        }
        else if (resolution.value == 1)
        {
            Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        }
        else if (resolution.value == 2)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }

        StartCoroutine(nameof(ResolutionSetting_co));
    }

    IEnumerator ResolutionSetting_co()
    {
        resolutionWindow.SetActive(true);

        for (int i = 0; i < 5; i++)
        {
            resolutionCount.text = $"{5 - i}";
            yield return new WaitForSeconds(1);
        }

        int resolutionValue = DataManager.instance.LoadResolution();

        resolutionWindow.SetActive(false);
        isResolution = false;

        if (resolutionValue == 0)
        {
            Screen.SetResolution(1920, 1080, false);
        }
        else if (resolutionValue == 1)
        {
            Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        }
        else if (resolutionValue == 2)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }

        resolution.value = resolutionValue;

        yield return new WaitForSeconds(0.5f);
        isResolution = true;
    }

    public void CheckButton()
    {
        StopCoroutine(nameof(ResolutionSetting_co));
        resolutionWindow.SetActive(false);
        DataManager.instance.SaveResolution(resolution.value);
    }

    public void ContinuBtnOnOff()
    {
        for (int i = 1; i < 4; i++)
        {
            string filePath = Application.persistentDataPath + "/PlayerData" + i + ".xml";

            if (File.Exists(filePath))
            {
                onContinu.SetActive(true);
                offContinu.SetActive(false);
                break;
            }
            onContinu.SetActive(false);
            offContinu.SetActive(true);
        }
    }

    public void NewGameButton()
    {
        DataManager.instance.NewGameSlot();
        if (DataManager.instance.saveNumber != 0)
        {
            AudioManager.instance.StopBGM();
            LoadingSceneManager.Instance.LoadScene("PlayerCustomizeTest");
        }
        else
        {
            image_Message.SetActive(true);
        }
    }

    public void MessageClose()
    {
        image_Message.SetActive(false);
    }

    public void ContinuGameButton()
    {
        image_DataSlot.SetActive(true);
        for(int i = 1; i < 4; i++)
        {
            string filePath = Application.persistentDataPath + "/PlayerData" + i + ".xml";

            if (File.Exists(filePath))
            {
                PlayerData playerData = DataManager.instance.PlayerDataGet(i);

                playerInpomation[i - 1].text =
                    "이름 : " + playerData.playerName + "\n\n" +
                    "직업 : " + playerData.job + "\n\n" +
                    "레벨 : " + playerData.playerLevel + "\n\n" +
                    "공격력 : " + playerData.staters.attack + "\n\n" +
                    "방어력 : " + playerData.staters.defens;
            }
            else
            {
                playerInpomation[i - 1].text =
                    "이름 : OOOO" + "\n\n" +
                    "직업 : OOOO" + "\n\n" +
                    "레벨 : 0" + "\n\n" +
                    "공격력 : 0" + "\n\n" +
                    "방어력 : 0";
            }
        }
    }

    public void SlotDelete(int num)
    {
        image_DeleteData.SetActive(true);

        saveDataNumber = num;
    }

    public void DeleteCheck()
    {
        DataManager.instance.DeleteData(saveDataNumber);

        for (int i = 1; i < 4; i++)
        {
            string filePath = Application.persistentDataPath + "/PlayerData" + i + ".xml";

            if (File.Exists(filePath))
            {
                PlayerData playerData = DataManager.instance.PlayerDataGet(i);

                playerInpomation[i - 1].text =
                    "이름 : " + playerData.playerName + "\n\n" +
                    "직업 : " + playerData.job + "\n\n" +
                    "레벨 : " + playerData.playerLevel + "\n\n" +
                    "공격력 : " + playerData.staters.attack + "\n\n" +
                    "방어력 : " + playerData.staters.defens;
            }
            else
            {
                playerInpomation[i - 1].text =
                    "이름 : OOOO" + "\n\n" +
                    "직업 : OOOO" + "\n\n" +
                    "레벨 : 0" + "\n\n" +
                    "공격력 : 0" + "\n\n" +
                    "방어력 : 0";
            }
        }

        image_DeleteData.SetActive(false);
    }

    public void DeleteClose()
    {
        image_DeleteData.SetActive(false);
    }

    public void CopntinuSlotClose()
    {
        image_DataSlot.SetActive(false);
        ContinuBtnOnOff();
    }

    public void SlotClick(int num)
    {
        DataManager.instance.saveNumber = num;
        LoadingSceneManager.Instance.LoadScene("In Game");
    }
}
