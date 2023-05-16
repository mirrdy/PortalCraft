using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TitleUiManager : MonoBehaviour
{
    [Header("Windows")]
    [SerializeField] GameObject settingWindow;

    [Header("Setting Window")]
    [SerializeField] TMP_Dropdown resolution;
    [SerializeField] Slider slider_Bgm;
    [SerializeField] Slider slider_Sfx;

    float bgmValue = 1f;
    float sfxValue = 1f;

    public void BGM_VolumeSetting()  // ����� �Ҹ� ����
    {
        AudioManager.instance.bgmPlay.volume = slider_Bgm.value;
    }

    public void SFX_VolumeSetting()  // ȿ���� �Ҹ� ����
    {
        for (int i = 0; i < AudioManager.instance.sfxPlay.Length; i++)
        {
            AudioManager.instance.sfxPlay[i].volume = slider_Sfx.value;
        }
    }

    public void SaveButton()  // ���� ��ư �޼ҵ�
    {
        bgmValue = slider_Bgm.value;
        sfxValue = slider_Sfx.value;
    }

    public void BackButton()  // �ڷΰ��� ��ư �޼ҵ�
    {
        slider_Bgm.value = bgmValue;
        slider_Sfx.value = sfxValue;
        settingWindow.SetActive(false);
    }

    public void CheckSound(BaseEventData eventdata)  // �����̴� �Ҹ� ����� ���� üũ �޼ҵ�
    {
        AudioManager.instance.PlaySFX("SoundCheck");
    }

    public void SettingWindow()  // �ɼ� â ȣ�� ��ư �޼ҵ�
    {
        settingWindow.SetActive(true);
    }

    public void GameQuit()  // ���� ���� �޼ҵ�
    {
        Application.Quit();
    }

    public void DropDownValue()
    {
        if(resolution.value == 0)
        {
            Screen.SetResolution(1920, 1080, false);
        }
        else if(resolution.value == 1)
        {
            Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        }
        else if(resolution.value == 2)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
    }
}
