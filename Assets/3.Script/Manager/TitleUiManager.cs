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
        bgmValue = slider_Bgm.value;
        sfxValue = slider_Sfx.value;
    }

    public void BackButton()  // 뒤로가기 버튼 메소드
    {
        slider_Bgm.value = bgmValue;
        slider_Sfx.value = sfxValue;
        settingWindow.SetActive(false);
    }

    public void CheckSound(BaseEventData eventdata)  // 슬라이더 소리 변경시 사운드 체크 메소드
    {
        AudioManager.instance.PlaySFX("SoundCheck");
    }

    public void SettingWindow()  // 옵션 창 호출 버튼 메소드
    {
        settingWindow.SetActive(true);
    }

    public void GameQuit()  // 게임 종료 메소드
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
