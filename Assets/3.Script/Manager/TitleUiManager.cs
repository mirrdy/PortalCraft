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

    private bool isResolution = false;

    private void Start()
    {
        slider_Bgm.value = DataManager.instance.LoadSound()[0];
        slider_Sfx.value = DataManager.instance.LoadSound()[1];
        resolution.value = DataManager.instance.LoadResolution();
    }

    public void BGM_VolumeSetting()  // 배경음 소리 설정
    {
        AudioManager.instance.bgmPlay.volume = slider_Bgm.value;

    }

    public void SFX_VolumeSetting()  // 효과음 소리 설정
    {
        for (int i = 0; i < AudioManager.instance.sfxPlay.Length; i++)
        {
            AudioManager.instance.sfxPlay[i].volume = slider_Sfx.value;
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
        if(!isResolution)
        {
            return;
        }

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

        StartCoroutine(nameof(ResolutionSetting_co));
    }

    IEnumerator ResolutionSetting_co()
    {
        resolutionWindow.SetActive(true);

        for (int i=0; i<5; i++)
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
}
