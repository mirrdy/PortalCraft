using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("Windows")]
    [SerializeField] GameObject settingWindow;

    [Header("Setting Window")]
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SFXSlider;

    float BGMValue = 1f;
    float SFXValue = 1f;

    public void BGM_VolumeSetting()
    {
        AudioManager.instance.BGMPlay.volume = BGMSlider.value;
    }

    public void SFX_VolumeSetting()
    {
        for (int i = 0; i < AudioManager.instance.SFX_Play.Length; i++)
        {
            AudioManager.instance.SFX_Play[i].volume = SFXSlider.value;
        }
    }

    public void SaveButton()
    {
        BGMValue = BGMSlider.value;
        SFXValue = SFXSlider.value;
    }

    public void BackButton() 
    {
        BGMSlider.value = BGMValue;
        SFXSlider.value = SFXValue;
        settingWindow.SetActive(false);
    }

    public void SoundCheck(BaseEventData eventdata)
    {
        AudioManager.instance.PlaySFX("SoundCheck");
    }
}
