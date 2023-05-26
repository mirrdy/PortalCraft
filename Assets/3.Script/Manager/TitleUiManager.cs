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

    public void BGM_VolumeSetting()  // ����� �Ҹ� ����
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

    public void SFX_VolumeSetting()  // ȿ���� �Ҹ� ����
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

    public void SaveButton()  // ���� ��ư �޼ҵ�
    {
        DataManager.instance.SaveSound(slider_Bgm.value, slider_Sfx.value);
    }

    public void BackButton()  // �ڷΰ��� ��ư �޼ҵ�
    {
        isResolution = false;

        slider_Bgm.value = DataManager.instance.LoadSound()[0];
        slider_Sfx.value = DataManager.instance.LoadSound()[1];
        settingWindow.SetActive(false);
    }

    public void CheckSound(BaseEventData eventdata)  // �����̴� �Ҹ� ����� ���� üũ �޼ҵ�
    {
        AudioManager.instance.PlaySFX("SoundCheck");
    }

    public void SettingWindow()  // �ɼ� â ȣ�� ��ư �޼ҵ�
    {
        isResolution = true;
        settingWindow.SetActive(true);
    }

    public void GameQuit()  // ���� ���� �޼ҵ�
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
                    "�̸� : " + playerData.playerName + "\n\n" +
                    "���� : " + playerData.job + "\n\n" +
                    "���� : " + playerData.playerLevel + "\n\n" +
                    "���ݷ� : " + playerData.staters.attack + "\n\n" +
                    "���� : " + playerData.staters.defens;
            }
            else
            {
                playerInpomation[i - 1].text =
                    "�̸� : OOOO" + "\n\n" +
                    "���� : OOOO" + "\n\n" +
                    "���� : 0" + "\n\n" +
                    "���ݷ� : 0" + "\n\n" +
                    "���� : 0";
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
                    "�̸� : " + playerData.playerName + "\n\n" +
                    "���� : " + playerData.job + "\n\n" +
                    "���� : " + playerData.playerLevel + "\n\n" +
                    "���ݷ� : " + playerData.staters.attack + "\n\n" +
                    "���� : " + playerData.staters.defens;
            }
            else
            {
                playerInpomation[i - 1].text =
                    "�̸� : OOOO" + "\n\n" +
                    "���� : OOOO" + "\n\n" +
                    "���� : 0" + "\n\n" +
                    "���ݷ� : 0" + "\n\n" +
                    "���� : 0";
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
