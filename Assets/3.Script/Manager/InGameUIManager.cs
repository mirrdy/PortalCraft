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

    private bool isResolution = false;

    private ItemManager itemInfo;
    private SkillManager skillInfo;

    private void Start()
    {
        slider_Bgm.value = DataManager.instance.LoadSound()[0];
        slider_Sfx.value = DataManager.instance.LoadSound()[1];
        resolution.value = DataManager.instance.LoadResolution();
        itemInfo = FindObjectOfType<ItemManager>();
        skillInfo = FindObjectOfType<SkillManager>();
    }

    private void OnEnable()
    {
        menuImage.SetActive(false);
        settingMenu.SetActive(false);
        resolutionWindow.SetActive(false);
        image_Staters.SetActive(false);
        inventory.SetActive(false);
    }

    private void Update()
    {
        MenuOnOff();
        StatersOnOff();
        InventoryOnOff();
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

    public void MenuOnOff()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuImage.activeSelf)
            {
                menuImage.SetActive(true);
                image_Staters.SetActive(false);
                inventory.SetActive(false);
            }
            else
            {
                menuImage.SetActive(false);
            }
        }
    }

    public void ContinuGameBtn()
    {
        menuImage.SetActive(false);
    }

    public void SaveBtn()
    {
        DataManager.instance.SaveData(PlayerControl.instance.playerData, DataManager.instance.saveNumber);
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

    public void StatersOnOff()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            if(!image_Staters.activeSelf)
            {
                OnSkillStatersCall();
                image_Staters.SetActive(true);
                inventory.SetActive(false);
                menuImage.SetActive(false);
            }
            else
            {
                image_Staters.SetActive(false);
            }
        }
    }

    public void OnSkillStatersCall()
    {
        PlayerData playerData = PlayerControl.instance.playerData;
        
        if(playerData.job.Equals("����"))
        {
            skillImage[0].sprite = sprite_Skill[0];
            skillImage[1].sprite = sprite_Skill[1];
            playerData.skill[0].skillNum = 3;
            playerData.skill[1].skillNum = 4;
        }
        else
        {
            skillImage[0].sprite = sprite_Skill[2];
            skillImage[1].sprite = sprite_Skill[3];
            playerData.skill[0].skillNum = 1;
            playerData.skill[1].skillNum = 2;
        }

        SkillCheck();

        StatersCheck();
    }

    private void SkillCheck()
    {
        PlayerData playerData = PlayerControl.instance.playerData;

        skillPoint.text = "��ų ����Ʈ : " + playerData.staters.skillPoint;

        int skillNum = 0;

        for (int i = 0; i < skillLevel.Length; i++)
        {
            skillLevel[i].text = "���� : " + playerData.skill[0].skillLevel;

            if (playerData.skill[i].skillLevel > 0)
            {
                for (int k = 0; k < skillInfo.list_Skill.Count; k++)
                {
                    if (skillInfo.list_Skill[k].level == playerData.skill[i].skillLevel && skillInfo.list_Skill[i].tag == playerData.skill[i].skillNum)
                    {
                        skillNum = k;
                    }
                }
                skillTooltip[i].text = "������ : " + skillInfo.list_Skill[skillNum].damage + "\n" +
                                        "��Ÿ�� : " + skillInfo.list_Skill[skillNum].coolTime + " (S)\n" +
                                        "���� : \n" + skillInfo.list_Skill[skillNum].tooltip;
                skillDown[i].interactable = true;
            }
            else
            {
                skillTooltip[i].text = "������ : " + " " + "\n" +
                                        "��Ÿ�� : " + " " + "\n" +
                                        "���� : \n" + " ";

                skillDown[i].interactable = false;
            }

            if (playerData.staters.skillPoint >= skillInfo.list_Skill[skillNum + 1].skillUpPoint && playerData.playerLevel >= skillInfo.list_Skill[skillNum].levelLimit && playerData.skill[i].skillLevel < 3)
            {
                skillUp[i].interactable = true;
            }
            else
            {
                skillUp[i].interactable = false;
            }
        }
    }

    private void StatersCheck()
    {
        PlayerData playerData = PlayerControl.instance.playerData;

        int count = playerData.playerLevel * 5 - playerData.staters.statersPoint;

        if(count > 0)
        {
            for(int i = 0; i < statDown.Length; i++)
            {
                statDown[i].interactable = true;
            }

            if(playerData.staters.maxHp <= 100)
            {
                statDown[0].interactable = false;
            }
            if (playerData.staters.maxMp <= 100)
            {
                statDown[1].interactable = false;
            }
            if (playerData.staters.attack <= 10)
            {
                statDown[2].interactable = false;
            }
            if (playerData.staters.defens <= 5)
            {
                statDown[3].interactable = false;
            }
        }
        else
        {
            for (int i = 0; i < statDown.Length; i++)
            {
                statDown[i].interactable = false;
            }
        }

        playerStat.text = "Player Staters\n\n" +
                            "�̸� : " + playerData.playerName + "\n" +
                            "���� : " + playerData.playerLevel + "\n" +
                            "ü�� : " + playerData.playerLevel + "\n" +
                            "���� : " + playerData.playerLevel + "\n" +
                            "���ݷ� : " + playerData.playerLevel + "\n" +
                            "���� : " + playerData.playerLevel + "\n" +
                            "���� �ӵ� : " + playerData.playerLevel + "\n" +
                            "�̵� �ӵ� : " + playerData.playerLevel;

        curruntStat.text = " " + playerData.playerLevel + "\n" +
                            " " + playerData.playerLevel + "\n" +
                            " " + playerData.playerLevel + "\n" +
                            " " + playerData.playerLevel + "\n" +
                            " " + playerData.staters.statersPoint;
    }

    public void OneSkillUpDown(int value)
    {
        PlayerData playerData = PlayerControl.instance.playerData;

        playerData.skill[0].skillLevel += value;

        if(playerData.skill[0].skillLevel > 0)
        {
            playerData.skill[0].hasSkill = true;
        }
        else
        {
            playerData.skill[0].hasSkill = false;
        }

        SkillCheck();
    }

    public void TwoSkillUpDown(int value)
    {
        PlayerData playerData = PlayerControl.instance.playerData;

        playerData.skill[1].skillLevel += value;

        if (playerData.skill[1].skillLevel > 0)
        {
            playerData.skill[1].hasSkill = true;
        }
        else
        {
            playerData.skill[1].hasSkill = false;
        }

        SkillCheck();
    }

    public void HpUpDown(int value)
    {
        PlayerData playerData = PlayerControl.instance.playerData;

        playerData.staters.maxHp += value;

        StatersCheck();
    }

    public void MpUpDown(int value)
    {
        PlayerData playerData = PlayerControl.instance.playerData;

        playerData.staters.maxMp += value;

        StatersCheck();
    }

    public void AttackUpDown(int value)
    {
        PlayerData playerData = PlayerControl.instance.playerData;

        playerData.staters.maxMp += value;

        StatersCheck();
    }

    public void DeffenskUpDown(int value)
    {
        PlayerData playerData = PlayerControl.instance.playerData;

        playerData.staters.maxMp += value;

        StatersCheck();
    }

    private void InventoryOnOff()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(!inventory.activeSelf)
            {
                inventory.SetActive(true);
                image_Staters.SetActive(false);
                menuImage.SetActive(false);
            }
            else
            {
                inventory.SetActive(false);
            }
        }
    }
}
