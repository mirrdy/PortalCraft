using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InGameUIManager : MonoBehaviour
{
    [Header("Player UI")]
    [SerializeField] TMP_Text timer;  // 게임 타이머
    [SerializeField] Image hpBar;  // hp바
    [SerializeField] Text hpCheck;  // hp 표시
    [SerializeField] Image mpBar;  // mp바
    [SerializeField] Text mpCheck;  // mp 표시
    [SerializeField] Text player_Level;  // Player 레벨
    [SerializeField] Slider expBar;  // 경험치 바
    [SerializeField] Text expCheck;  // 경험치 양 표시

    [Header("Menu")]
    [SerializeField] GameObject menuImage;  // 메뉴 이미지
    [SerializeField] GameObject settingMenu;  // 설정창
    [SerializeField] Slider slider_Bgm;  // bgm 바
    [SerializeField] Slider slider_Sfx;  // sfx 바
    [SerializeField] GameObject resolutionWindow;  // 해상도 변경 시 표시할 창
    [SerializeField] TMP_Text resolutionCount;  // 해상도 변경 창 타이머
    [SerializeField] TMP_Dropdown resolution;

    [Header("Menu Icon")]
    [SerializeField] Image image_Bgm;  // 변경될 bgm 이미지
    [SerializeField] Image image_Sfx;  // 변경될 sfx 이미지
    [SerializeField] Sprite[] sprite_Bgm;  // bgm 이미지 변경할 스프라이트
    [SerializeField] Sprite[] sprite_Sfx;  // sfx 이미지 변경할 스프라이트

    [Header("Skill")]
    [SerializeField] GameObject[] image_Skill;  // 스킬 쿨타임 창
    [SerializeField] Image[] slider_Skill;  // 스킬 쿨타임 시계 방향 이미지
    [SerializeField] Text[] skillTimer;  // 스킬 쿨 남은 시간 표시
    [SerializeField] Sprite[] sprite_Skill;  // 스킬 아이콘

    [Header("Inventory")]
    [SerializeField] GameObject inventory;  // 인벤토리 창
    [SerializeField] Image[] itemFrame;  // 아이템 종류에 따른 프레임
    [SerializeField] Image[] itemSlot;  // 아이템 이미지 들어갈 변수
    [SerializeField] GameObject[] itemCount;  // 아이템 수량 표시 창
    [SerializeField] Text[] itemQuantity;  // 아이템 수량 표시
    [SerializeField] Sprite[] frameColor;  // 아이템 종류에 따른 프레임 컬러 보관 스프라이트

    [Header("Player Stateers")]
    [SerializeField] Text playerStat;

    PlayerController player = new PlayerController();

    private bool isResolution = false;

    private void Start()
    {
        slider_Bgm.value = DataManager.instance.LoadSound()[0];
        slider_Sfx.value = DataManager.instance.LoadSound()[1];
        resolution.value = DataManager.instance.LoadResolution();
    }

    private void OnEnable()
    {
        menuImage.SetActive(false);
        settingMenu.SetActive(false);
        resolutionWindow.SetActive(false);
        inventory.SetActive(false);
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
        settingMenu.SetActive(false);
    }

    public void CheckSound(BaseEventData eventdata)  // 슬라이더 소리 변경시 사운드 체크 메소드
    {
        AudioManager.instance.PlaySFX("SoundCheck");
    }

    public void OnMenu()
    {
        menuImage.SetActive(true);
    }

    public void ContinuGameBtn()
    {
        menuImage.SetActive(false);
    }

    public void SaveBtn()
    {
        DataManager.instance.SaveData(player.playerData, DataManager.instance.saveNumber);
    }

    public void SettingWindow()  // 옵션 창 호출 버튼 메소드
    {
        isResolution = true;
        settingMenu.SetActive(true);
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

    public void BackTitleSceneBtn()
    {
        LoadingSceneManager.Instance.LoadScene("Title");
    }
}
