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
    [SerializeField] GameObject playerView;  // 캐릭터 비추는 카메라

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
    [SerializeField] Text invenStatus;  // 장비창 플레이어 스테이터스 표시
    [SerializeField] Image[] itemBorder;  // 슬롯 테두리 이미지 
    [SerializeField] Image[] itemFrame;  // 아이템 종류에 따른 프레임
    [SerializeField] Image[] itemSlot;  // 아이템 이미지 들어갈 변수
    [SerializeField] GameObject[] itemCount;  // 아이템 수량 표시 창
    [SerializeField] Text[] itemQuantity;  // 아이템 수량 표시
    [SerializeField] Sprite[] frameColor;  // 아이템 종류에 따른 프레임 컬러 보관 스프라이트
    [SerializeField] Sprite[] image_Item;  // 아이템 2D스프라이트
    [SerializeField] GameObject image_Tooltip;  // 아이템 툴팁;
    [SerializeField] Text tooltip;  // 아이템 설명 표시
    [SerializeField] Sprite[] border;  // 슬롯에 마우스 올라갈때 테두리 변경용 이미지

    [Header("Player Status")]
    [SerializeField] GameObject image_Status;  // 스테이터스 창
    [SerializeField] Text playerStat;  // 플레이어 스탯 표시
    [SerializeField] Text curruntStat;  // 플레이어 스탯 올리고 내리는 텍스트
    [SerializeField] Button[] statUp;  // 스탯 업 버튼
    [SerializeField] Button[] statDown;  // 스탯 다운 버튼
    [SerializeField] Image[] skillImage;  // 직업에 맞는 스킬 표시
    [SerializeField] Text[] skillLevel;  // 현재 스킬 레벨 표시
    [SerializeField] Text[] skillTooltip; // 현재 스킬 정보 표시
    [SerializeField] Button[] skillUp;  // 스킬 레벨 올리는 버튼
    [SerializeField] Button[] skillDown;  // 스킬 레벨 내리는 버튼
    [SerializeField] Text skillPoint; // 현재 스킬 포인트 표시

    private bool isResolution = false;

    private ItemManager itemInfo;
    private SkillManager skillInfo;

    public int PlayerHand = 30;

    private Coroutine fillCoroutine;

    private void Start()
    {
        slider_Bgm.value = DataManager.instance.LoadSound()[0];
        slider_Sfx.value = DataManager.instance.LoadSound()[1];
        resolution.value = DataManager.instance.LoadResolution();
        itemInfo = FindObjectOfType<ItemManager>();
        skillInfo = FindObjectOfType<SkillManager>();

        for (int i = 1; i < 8; i++)
        {
            itemBorder[30 + i].sprite = border[0];
        }
        itemBorder[30].sprite = border[1];
    }

    private void OnEnable()
    {
        menuImage.SetActive(false);
        settingMenu.SetActive(false);
        resolutionWindow.SetActive(false);
        image_Status.SetActive(false);
        inventory.SetActive(false);
        playerView.SetActive(false);
        image_Tooltip.SetActive(false);
    }

    private void Update()
    {
        MenuOnOff();
        StatusOnOff();
        InventoryOnOff();
        InventoryCheck();
        SetQuickSlot();
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

    public void MenuOnOff()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuImage.activeSelf)
            {
                SetCursorState(false);
                Time.timeScale = 0;
                menuImage.SetActive(true);
                image_Status.SetActive(false);
                inventory.SetActive(false); 
                playerView.SetActive(false);
            }
            else
            {
                image_Tooltip.SetActive(false);
                SetCursorState(true);
                Time.timeScale = 1;
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

    public void StatusOnOff()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            if (!image_Status.activeSelf)
            {
                SetCursorState(false);
                Time.timeScale = 0;
                OnSkillStatusCall();
                image_Status.SetActive(true);
                inventory.SetActive(false);
                menuImage.SetActive(false);
                playerView.SetActive(true);
            }
            else
            {
                image_Tooltip.SetActive(false);
                SetCursorState(true);
                image_Status.SetActive(false);
                playerView.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }

    public void OnSkillStatusCall()
    {
        PlayerData playerData = PlayerControl.instance.playerData;
        
        if(playerData.job.Equals("전사"))
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

        StatusCheck();
    }

    private void SkillCheck()
    {
        PlayerData playerData = PlayerControl.instance.playerData;

        skillPoint.text = "스킬 포인트 : " + playerData.status.skillPoint;

        int skillNum = 0;

        for (int i = 0; i < skillLevel.Length; i++)
        {
            skillLevel[i].text = "레벨 : " + playerData.skill[0].skillLevel;

            if (playerData.skill[i].skillLevel > 0)
            {
                for (int k = 0; k < skillInfo.list_Skill.Count; k++)
                {
                    if (skillInfo.list_Skill[k].level == playerData.skill[i].skillLevel && skillInfo.list_Skill[i].tag == playerData.skill[i].skillNum)
                    {
                        skillNum = k;
                    }
                }
                skillTooltip[i].text = "데미지 : " + skillInfo.list_Skill[skillNum].damage + "\n" +
                                        "쿨타임 : " + skillInfo.list_Skill[skillNum].coolTime + " (S)\n" +
                                        "설명 : \n" + skillInfo.list_Skill[skillNum].tooltip;
                skillDown[i].interactable = true;
            }
            else
            {
                skillTooltip[i].text = "데미지 : " + " " + "\n" +
                                        "쿨타임 : " + " " + "\n" +
                                        "설명 : \n" + " ";

                skillDown[i].interactable = false;
            }

            if (playerData.status.skillPoint >= skillInfo.list_Skill[skillNum + 1].skillUpPoint && playerData.playerLevel >= skillInfo.list_Skill[skillNum].levelLimit && playerData.skill[i].skillLevel < 3)
            {
                skillUp[i].interactable = true;
            }
            else
            {
                skillUp[i].interactable = false;
            }
        }
    }

    private void StatusCheck()
    {
        PlayerData playerData = PlayerControl.instance.playerData;

        int count = playerData.playerLevel * 5 - playerData.status.statusPoint;

        if(count > 0)
        {
            for(int i = 0; i < statDown.Length; i++)
            {
                statDown[i].interactable = true;
            }

            if(playerData.status.maxHp <= 100)
            {
                statDown[0].interactable = false;
            }
            if (playerData.status.maxMp <= 100)
            {
                statDown[1].interactable = false;
            }
            if (playerData.status.attack <= 10)
            {
                statDown[2].interactable = false;
            }
            if (playerData.status.defens <= 5)
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

        if(playerData.status.statusPoint > 0)
        {
            for (int i = 0; i < statUp.Length; i++)
            {
                statUp[i].interactable = true;
            }
        }
        else
        {
            for (int i = 0; i < statUp.Length; i++)
            {
                statUp[i].interactable = false;
            }
        }

        playerStat.text = "Player Status\n\n" +
                            "이름 : " + playerData.playerName + "\n" +
                            "레벨 : " + playerData.playerLevel + "\n" +
                            "체력 : " + playerData.playerLevel + "\n" +
                            "마나 : " + playerData.playerLevel + "\n" +
                            "공격력 : " + playerData.playerLevel + "\n" +
                            "방어력 : " + playerData.playerLevel + "\n" +
                            "공격 속도 : " + playerData.playerLevel + "\n" +
                            "이동 속도 : " + playerData.playerLevel;

        curruntStat.text = " " + playerData.playerLevel + "\n" +
                            " " + playerData.playerLevel + "\n" +
                            " " + playerData.playerLevel + "\n" +
                            " " + playerData.playerLevel + "\n" +
                            " " + playerData.status.statusPoint;

        invenStatus.text = "Player Status\n" +
                            "체력 : " + playerData.playerLevel + "\n" +
                            "마나 : " + playerData.playerLevel + "\n" +
                            "이동 속도 : " + playerData.playerLevel + "\n" +
                            "공격 속도 : " + playerData.playerLevel + "\n" +
                            "공격력 : " + playerData.playerLevel + "\n" +
                            "방어력 : " + playerData.playerLevel;
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

        if (value > 0)
        {
            for (int k = 0; k < skillInfo.list_Skill.Count; k++)
            {
                if (skillInfo.list_Skill[k].level == playerData.skill[0].skillLevel && skillInfo.list_Skill[k].tag == playerData.skill[0].skillNum)
                {
                    playerData.status.skillPoint -= skillInfo.list_Skill[k + 1].skillUpPoint;
                }
            }
        }
        else
        {
            for (int k = 0; k < skillInfo.list_Skill.Count; k++)
            {
                if (skillInfo.list_Skill[k].level == playerData.skill[0].skillLevel && skillInfo.list_Skill[k].tag == playerData.skill[0].skillNum)
                {
                    playerData.status.skillPoint += skillInfo.list_Skill[k].skillUpPoint;
                }
            }
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

        if (value > 0)
        {
            for (int k = 0; k < skillInfo.list_Skill.Count; k++)
            {
                if (skillInfo.list_Skill[k].level == playerData.skill[0].skillLevel && skillInfo.list_Skill[k].tag == playerData.skill[1].skillNum)
                {
                    playerData.status.skillPoint -= skillInfo.list_Skill[k + 1].skillUpPoint;
                }
            }
        }
        else
        {
            for (int k = 0; k < skillInfo.list_Skill.Count; k++)
            {
                if (skillInfo.list_Skill[k].level == playerData.skill[1].skillLevel && skillInfo.list_Skill[k].tag == playerData.skill[1].skillNum)
                {
                    playerData.status.skillPoint += skillInfo.list_Skill[k].skillUpPoint;
                }
            }
        }

        SkillCheck();
    }

    public void HpUpDown(int value)
    {
        PlayerData playerData = PlayerControl.instance.playerData;

        playerData.status.maxHp += value;

        if (value > 0)
        {
            playerData.status.statusPoint--;
        }
        else
        {
            playerData.status.statusPoint++;
        }

        StatusCheck();
    }

    public void MpUpDown(int value)
    {
        PlayerData playerData = PlayerControl.instance.playerData;

        playerData.status.maxMp += value;

        if(value > 0)
        {
            playerData.status.statusPoint--;
        }
        else
        {
            playerData.status.statusPoint++;
        }

        StatusCheck();
    }

    public void AttackUpDown(int value)
    {
        PlayerData playerData = PlayerControl.instance.playerData;

        playerData.status.attack += value;

        if (value > 0)
        {
            playerData.status.statusPoint--;
        }
        else
        {
            playerData.status.statusPoint++;
        }

        StatusCheck();
    }

    public void DeffenskUpDown(int value)
    {
        PlayerData playerData = PlayerControl.instance.playerData;

        playerData.status.defens += value;

        if (value > 0)
        {
            playerData.status.statusPoint--;
        }
        else
        {
            playerData.status.statusPoint++;
        }

        StatusCheck();
    }

    private void InventoryOnOff()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(!inventory.activeSelf)
            {
                SetCursorState(false);
                Time.timeScale = 0;
                inventory.SetActive(true);
                image_Status.SetActive(false);
                menuImage.SetActive(false);
                playerView.SetActive(true);
            }
            else
            {
                image_Tooltip.SetActive(false);
                SetCursorState(true);
                Time.timeScale = 1;
                inventory.SetActive(false);
                playerView.SetActive(false);
            }
        }

        StatusCheck();
    }

    public void MouseEnter(int value)
    {
        PlayerData playerData = PlayerControl.instance.playerData;

        if(playerData.inventory[value].hasItem)
        {
            int num = 0;
            int tag = playerData.inventory[value].tag;

            image_Tooltip.SetActive(true);
            image_Tooltip.transform.position = itemSlot[value].transform.position + (Vector3.right * 5);

            for (int i = 0; i < itemInfo.list_AllItem.Count; i++)
            {
                if (tag == itemInfo.list_AllItem[i].tag)
                {
                    num = i;
                    break;
                }
            }

            if(itemInfo.list_AllItem[num].type.Equals("Block"))
            {
                for (int i = 0; i < itemInfo.list_Block.Count; i++)
                {
                    if (tag == itemInfo.list_Block[i].tag)
                    {
                        tooltip.text = "이름 : " + itemInfo.list_Block[i].name + "\n" +
                                        "수량 : " + playerData.inventory[value].quantity + "\n" +
                                        "-------------\n" +
                                        "" + itemInfo.list_Block[i].tooltip;
                        break;
                    }
                }
            }
            else if (itemInfo.list_AllItem[num].type.Equals("Armor"))
            {
                for (int i = 0; i < itemInfo.list_Armor.Count; i++)
                {
                    if (tag == itemInfo.list_Armor[i].tag)
                    {
                        tooltip.text = "이름 : " + itemInfo.list_Armor[i].name + "\n" +
                                "수량 : " + playerData.inventory[value].quantity + "\n" +
                                "-------------\n" +
                                "체력 : " + itemInfo.list_Armor[i].hp + "\n" +
                                "이동 속도 : " + itemInfo.list_Armor[i].moveSpeed + "\n" +
                                "-------------\n" +
                                "" + itemInfo.list_Armor[i].tooltip;
                        break;
                    }
                }
            }
            else if (itemInfo.list_AllItem[num].type.Equals("Helmet"))
            {
                for (int i = 0; i < itemInfo.list_Helmet.Count; i++)
                {
                    if (tag == itemInfo.list_Helmet[i].tag)
                    {
                        tooltip.text = "이름 : " + itemInfo.list_Helmet[i].name + "\n" +
                                "수량 : " + playerData.inventory[value].quantity + "\n" +
                                "-------------\n" +
                                "체력 : " + itemInfo.list_Helmet[i].defens + "\n" +
                                "-------------\n" +
                                "" + itemInfo.list_Helmet[i].tooltip;
                        break;
                    }
                }
            }
            else if (itemInfo.list_AllItem[num].type.Equals("Arms"))
            {
                for (int i = 0; i < itemInfo.list_Arms.Count; i++)
                {
                    if (tag == itemInfo.list_Arms[i].tag)
                    {
                        tooltip.text = "이름 : " + itemInfo.list_Arms[i].name + "\n" +
                                "수량 : " + playerData.inventory[value].quantity + "\n" +
                                "-------------\n" +
                                "공격력 : " + itemInfo.list_Arms[i].attack + "\n" +
                                "공격 속도 : " + itemInfo.list_Arms[i].attackSpeed + "\n" +
                                "-------------\n" +
                                "" + itemInfo.list_Arms[i].tooltip;
                        break;
                    }
                }
            }
            else if (itemInfo.list_AllItem[num].type.Equals("Cloak"))
            {
                tooltip.text = "이름 : " + itemInfo.list_Cloak[0].name + "\n" +
                        "수량 : " + playerData.inventory[value].quantity + "\n" +
                        "-------------\n" +
                        "" + itemInfo.list_Cloak[0].tooltip;
            }
            else if (itemInfo.list_AllItem[num].type.Equals("Etc"))
            {
                for (int i = 0; i < itemInfo.list_Etc.Count; i++)
                {
                    if (tag == itemInfo.list_Etc[i].tag)
                    {
                        tooltip.text = "이름 : " + itemInfo.list_Etc[i].name + "\n" +
                                "수량 : " + playerData.inventory[value].quantity + "\n" +
                                "-------------\n" +
                                "" + itemInfo.list_Etc[i].tooltip;
                        break;
                    }
                }
            }
            else if (itemInfo.list_AllItem[num].type.Equals("About"))
            {
                for (int i = 0; i < itemInfo.list_About.Count; i++)
                {
                    if (tag == itemInfo.list_About[i].tag)
                    {
                        tooltip.text = "이름 : " + itemInfo.list_About[i].name + "\n" +
                                "수량 : " + playerData.inventory[value].quantity + "\n" +
                                "-------------\n" +
                                "회복량 : " + itemInfo.list_About[i].recovery + "\n" +
                                "-------------\n" +
                                "" + itemInfo.list_About[i].tooltip;
                        break;
                    }
                }
            }
        }
    }

    public void MouseExit()
    {
        image_Tooltip.SetActive(false);
    }

    public void BorderChange(int value)
    {
        for (int i = 0; i < itemBorder.Length - 8; i++)
        {
            if (value == i)
            {
                itemBorder[value].sprite = border[1];
                continue;
            }
            itemBorder[i].sprite = border[0];
        }
    }

    private void SetCursorState(bool cursorState)
    {
        Cursor.lockState = cursorState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void InventoryCheck()
    {
        PlayerData playerData = PlayerControl.instance.playerData;

        for (int i = 0; i < playerData.inventory.Length; i++)
        {
            if (playerData.inventory[i].hasItem)
            {
                for (int k = 0; k < itemInfo.list_AllItem.Count; k++)
                {
                    if (playerData.inventory[i].tag == itemInfo.list_AllItem[k].tag)
                    {
                        itemSlot[i].gameObject.SetActive(true);
                        itemSlot[i].sprite = image_Item[k];

                        if(itemInfo.list_AllItem[k].maxQuantity > 2)
                        {
                            itemCount[i].SetActive(true);
                            itemQuantity[i].text = "" + playerData.inventory[i].quantity;
                        }
                        else
                        {
                            itemCount[i].SetActive(false);
                        }

                        if(itemInfo.list_AllItem[k].Equals("Helmet") || itemInfo.list_AllItem[k].Equals("Armor") || itemInfo.list_AllItem[k].Equals("Arms"))
                        {
                            itemFrame[i].sprite = frameColor[2];
                        }
                        else if(itemInfo.list_AllItem[k].Equals("About"))
                        {
                            itemFrame[i].sprite = frameColor[1];
                        }
                        else if (itemInfo.list_AllItem[k].Equals("Block"))
                        {
                            itemFrame[i].sprite = frameColor[3];
                        }

                        break;
                    }
                }
            }
            else
            {
                itemSlot[i].gameObject.SetActive(false);
                if (i < 38)
                {
                    itemFrame[i].sprite = frameColor[0];
                }
            }
        }
    }
    
    public void SetQuickSlot()
    {
        #region 퀵슬롯 슬롯 번호 배정
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerHand = 30;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerHand = 31;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayerHand = 32;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayerHand = 33;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayerHand = 34;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            PlayerHand = 35;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            PlayerHand = 36;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            PlayerHand = 37;
        }

        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (wheelInput > 0)
        {
            PlayerHand--;
            if(PlayerHand < 30)
            {
                PlayerHand = 37;
            }
        }
        else if (wheelInput < 0)
        {
            PlayerHand++;
            if (PlayerHand > 37)
            {
                PlayerHand = 30;
            }
        }

        for (int i = 0; i < 8; i++)
        {
            itemBorder[30 + i].sprite = border[0];
        }
        itemBorder[PlayerHand].sprite = border[1];
        #endregion
    }

    public void HpCheck(int maxHp, int currentHp)
    {
        float goals = currentHp / (float)maxHp;

        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine); // 기존 코루틴 종료
        }

        fillCoroutine = StartCoroutine(HpMpDelay_co(goals));
        hpCheck.text = currentHp + " / " + maxHp;
    }

    IEnumerator HpMpDelay_co(float goals)
    {
        float timer = 0f;
        float current = hpBar.fillAmount;
        float duration = 1f; // 체력 감소 지속 시간

        while (timer <= duration)
        {
            yield return null;
            timer += Time.deltaTime;
            float t = timer / duration;
            hpBar.fillAmount = Mathf.Lerp(current, goals, t);
        }

        hpBar.fillAmount = goals;
        fillCoroutine = null;
    }
}
