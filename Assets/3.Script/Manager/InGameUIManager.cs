using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InGameUIManager : MonoBehaviour
{
    [Header("Player UI")]
    [SerializeField] TMP_Text timer;  // ���� Ÿ�̸�
    [SerializeField] Image hpBar;  // hp��
    [SerializeField] Text hpCheck;  // hp ǥ��
    [SerializeField] Image mpBar;  // mp��
    [SerializeField] Text mpCheck;  // mp ǥ��
    [SerializeField] Text player_Level;  // Player ����
    [SerializeField] Slider expBar;  // ����ġ ��
    [SerializeField] Text expCheck;  // ����ġ �� ǥ��

    [Header("Menu")]
    [SerializeField] GameObject menuImage;  // �޴� �̹���
    [SerializeField] GameObject settingMenu;  // ����â
    [SerializeField] Slider slider_Bgm;  // bgm ��
    [SerializeField] Slider slider_Sfx;  // sfx ��
    [SerializeField] GameObject resolutionWindow;  // �ػ� ���� �� ǥ���� â
    [SerializeField] TMP_Text resolutionCount;  // �ػ� ���� â Ÿ�̸�
    [SerializeField] TMP_Dropdown resolution;

    [Header("Menu Icon")]
    [SerializeField] Image image_Bgm;  // ����� bgm �̹���
    [SerializeField] Image image_Sfx;  // ����� sfx �̹���
    [SerializeField] Sprite[] sprite_Bgm;  // bgm �̹��� ������ ��������Ʈ
    [SerializeField] Sprite[] sprite_Sfx;  // sfx �̹��� ������ ��������Ʈ

    [Header("Skill")]
    [SerializeField] GameObject[] image_Skill;  // ��ų ��Ÿ�� â
    [SerializeField] Image[] slider_Skill;  // ��ų ��Ÿ�� �ð� ���� �̹���
    [SerializeField] Text[] skillTimer;  // ��ų �� ���� �ð� ǥ��
    [SerializeField] Sprite[] sprite_Skill;  // ��ų ������

    [Header("Inventory")]
    [SerializeField] GameObject inventory;  // �κ��丮 â
    [SerializeField] Image[] itemFrame;  // ������ ������ ���� ������
    [SerializeField] Image[] itemSlot;  // ������ �̹��� �� ����
    [SerializeField] GameObject[] itemCount;  // ������ ���� ǥ�� â
    [SerializeField] Text[] itemQuantity;  // ������ ���� ǥ��
    [SerializeField] Sprite[] frameColor;  // ������ ������ ���� ������ �÷� ���� ��������Ʈ
    [SerializeField] Sprite[] image_Item;  // ������ 2D��������Ʈ

    [Header("Player Stateers")]
    [SerializeField] GameObject image_Staters;  // �������ͽ� â
    [SerializeField] Text playerStat;  // �÷��̾� ���� ǥ��
    [SerializeField] Text curruntStat;  // �÷��̾� ���� �ø��� ������ �ؽ�Ʈ
    [SerializeField] Button[] statUp;  // ���� �� ��ư
    [SerializeField] Button[] statDown;  // ���� �ٿ� ��ư
    [SerializeField] Image[] skillImage;  // ������ �´� ��ų ǥ��
    [SerializeField] Text[] skillLevel;  // ���� ��ų ���� ǥ��
    [SerializeField] Text[] skillTooltip; // ���� ��ų ���� ǥ��
    [SerializeField] Button[] skillUp;  // ��ų ���� �ø��� ��ư
    [SerializeField] Button[] skillDown;  // ��ų ���� ������ ��ư
    [SerializeField] Text skillPoint; // ���� ��ų ����Ʈ ǥ��

    PlayerController player;

    private bool isResolution = false;

    public delegate PlayerData PlayerControlDelegate();
    public PlayerControlDelegate playerContorldelegate;

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
        settingMenu.SetActive(false);
    }

    public void CheckSound(BaseEventData eventdata)  // �����̴� �Ҹ� ����� ���� üũ �޼ҵ�
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

    public void SettingWindow()  // �ɼ� â ȣ�� ��ư �޼ҵ�
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

    public void playerDataCall()
    {

    }
}
