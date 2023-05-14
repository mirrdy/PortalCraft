using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("Windows")]
    [SerializeField] GameObject settingWindow;

    [Header("Setting Window")]
    [SerializeField] Slider BGM_Slider;
    [SerializeField] Slider SFX_Slider;

    float bgmValue = 1f;
    float sfxValue = 1f;

    public void BGM_VolumeSetting()
    {
        AudioManager.instance.BGM_Play.volume = BGM_Slider.value;
    }

    public void SFX_VolumeSetting()
    {
        for (int i = 0; i < AudioManager.instance.SFX_Play.Length; i++)
        {
            AudioManager.instance.SFX_Play[i].volume = SFX_Slider.value;
        }
    }

    public void SaveButton()
    {
        bgmValue = BGM_Slider.value;
        sfxValue = SFX_Slider.value;
    }

    public void BackButton()
    {
        BGM_Slider.value = bgmValue;
        SFX_Slider.value = sfxValue;
        settingWindow.SetActive(false);
    }
}
