using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

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
    [SerializeField] Slot[] slotUi;

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

    [Header("Player Target Image")]
    [SerializeField] GameObject target;  // 플레이어 타켓 표시 용 오브젝트

    [Header("Current Slot")]
    [SerializeField] GameObject currentSlot;
    [SerializeField] Image currentImage;
    [SerializeField] GameObject text_currentSlot;
    [SerializeField] Text SlotItemCount;

    [Header("Prefab")]
    [SerializeField] GameObject[] prepab;  // 필드 드랍용 아이템 프리펩 배열

    private bool isResolution = false;

    private ItemManager itemInfo;
    private SkillManager skillInfo;

    private PlayerControl player;

    public int playerHand = 30;
    public int currentTag = 0;
    public int currentSlotNumber = 0;

    // 코루틴 저장할 변수
    private Coroutine hpCoroutine;
    private Coroutine mpCoroutine;
    private Coroutine expCoroutine;

    // 플레이어가 현재 퀵슬롯을 사용할지 인벤토리 상호 작용 인지 알수 있는 bool 값 변수
    public bool isQuickSlot = true;
    
    private void Reset()
    {
        #region 리셋
        if (GameObject.Find("Player") != null && GameObject.Find("Player").transform.childCount > 5)
        {
            playerView = GameObject.Find("Player").transform.GetChild(5).gameObject;
        }

        if (GameObject.Find("Player") != null && GameObject.Find("Player").transform.childCount > 4)
        {
            target = GameObject.Find("Player").transform.GetChild(4).gameObject;
        }

        Canvas objects_UI = GameObject.Find("InGame Canvas").GetComponent<Canvas>();

        Transform playerUI = objects_UI.transform.GetChild(0);
        Transform inventoryUI = objects_UI.transform.GetChild(1);
        Transform statusUI = objects_UI.transform.GetChild(2);
        Transform menuUI = objects_UI.transform.GetChild(3);
        Transform tooltipUI = objects_UI.transform.GetChild(4);
        Transform DragSlotUI = objects_UI.transform.GetChild(5);

        playerUI.GetChild(0).GetChild(0).TryGetComponent(out timer);
        playerUI.GetChild(2).GetChild(0).TryGetComponent(out hpBar);
        playerUI.GetChild(2).GetChild(1).TryGetComponent(out hpCheck);
        playerUI.GetChild(3).GetChild(0).TryGetComponent(out mpBar);
        playerUI.GetChild(3).GetChild(1).TryGetComponent(out mpCheck);
        playerUI.GetChild(4).GetChild(0).TryGetComponent(out player_Level);
        playerUI.GetChild(4).GetChild(1).TryGetComponent(out expBar);
        playerUI.GetChild(4).GetChild(1).GetChild(2).TryGetComponent(out expCheck);

        menuImage = menuUI.GetComponentInChildren<Transform>(true).gameObject;
        settingMenu = menuUI.GetChild(0).GetChild(5).GetComponentInChildren<Transform>(true).gameObject;
        slider_Bgm = menuUI.GetChild(0).GetChild(5).GetChild(1).GetComponentInChildren<Slider>(true);
        slider_Sfx = menuUI.GetChild(0).GetChild(5).GetChild(2).GetComponentInChildren<Slider>(true);
        resolutionWindow = menuUI.GetChild(0).GetChild(6).GetComponentInChildren<Transform>(true).gameObject;
        resolutionCount = menuUI.GetChild(0).GetChild(6).GetChild(2).GetComponentInChildren<TMP_Text>(true);
        resolution = menuUI.GetChild(0).GetChild(5).GetChild(5).GetComponentInChildren<TMP_Dropdown>(true);
        image_Bgm = menuUI.GetChild(0).GetChild(5).GetChild(1).GetChild(3).GetComponentInChildren<Image>(true);
        image_Sfx = menuUI.GetChild(0).GetChild(5).GetChild(2).GetChild(3).GetComponentInChildren<Image>(true);
        sprite_Bgm = new Sprite[2];
        sprite_Bgm[0] = Resources.Load<Sprite>("Sprites/Icon_PictoIcon_Sound_on");
        sprite_Bgm[1] = Resources.Load<Sprite>("Sprites/Icon_PictoIcon_Sound_off");
        sprite_Sfx = new Sprite[2];
        sprite_Sfx[0] = Resources.Load<Sprite>("Sprites/Icon_PictoIcon_Music_on");
        sprite_Sfx[1] = Resources.Load<Sprite>("Sprites/Icon_PictoIcon_Music_off");

        image_Skill = new GameObject[2];
        image_Skill[0] = playerUI.GetChild(5).GetChild(0).GetComponentInChildren<Transform>(true).gameObject;
        image_Skill[1] = playerUI.GetChild(5).GetChild(1).GetComponentInChildren<Transform>(true).gameObject;
        slider_Skill = new Image[2];
        slider_Skill[0] = playerUI.GetChild(5).GetChild(0).GetChild(0).GetComponentInChildren<Image>(true);
        slider_Skill[1] = playerUI.GetChild(5).GetChild(1).GetChild(0).GetComponentInChildren<Image>(true);
        skillTimer = new Text[2];
        skillTimer[0] = playerUI.GetChild(5).GetChild(0).GetChild(1).GetComponentInChildren<Text>(true);
        skillTimer[1] = playerUI.GetChild(5).GetChild(1).GetChild(1).GetComponentInChildren<Text>(true);
        sprite_Skill = new Sprite[4];
        sprite_Skill[0] = Resources.Load<Sprite>("Sprites/UI_Skill_Icon_Chop");
        sprite_Skill[1] = Resources.Load<Sprite>("Sprites/UI_Skill_Icon_Slash");
        sprite_Skill[2] = Resources.Load<Sprite>("Sprites/UI_Skill_Icon_Arrow_Barrage");
        sprite_Skill[3] = Resources.Load<Sprite>("Sprites/UI_Skill_Icon_Beam");

        inventory = inventoryUI.GetComponentInChildren<Transform>(true).gameObject;
        invenStatus = inventoryUI.GetChild(0).GetChild(1).GetChild(0).GetComponentInChildren<Text>(true);
        itemBorder = new Image[38];
        for(int i = 0; i < 30; i++)
        {
            itemBorder[i] = inventoryUI.GetChild(0).GetChild(0).GetChild(i).GetComponentInChildren<Image>(true);
        }
        for(int i = 30; i < 38; i++)
        {
            itemBorder[i] = playerUI.GetChild(1).GetChild(i - 30).GetComponentInChildren<Image>(true);
        }
        itemFrame = new Image[38];
        for (int i = 0; i < 30; i++)
        {
            itemFrame[i] = inventoryUI.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponentInChildren<Image>(true);
        }
        for (int i = 30; i < 38; i++)
        {
            itemFrame[i] = playerUI.GetChild(1).GetChild(i - 30).GetChild(0).GetComponentInChildren<Image>(true);
        }
        itemSlot = new Image[41];
        for (int i = 0; i < 30; i++)
        {
            itemSlot[i] = inventoryUI.GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponentInChildren<Image>(true);
        }
        for (int i = 30; i < 38; i++)
        {
            itemSlot[i] = playerUI.GetChild(1).GetChild(i - 30).GetChild(1).GetComponentInChildren<Image>(true);
        }
        for (int i = 38; i < 41; i++)
        {
            itemSlot[i] = inventoryUI.GetChild(0).GetChild(2).GetChild(i - 37).GetChild(1).GetComponentInChildren<Image>(true);
        }
        slotUi = new Slot[41];
        Slot[] slots = FindObjectsOfType<Slot>(true);
        Array.Sort(slots, (a, b) => string.Compare(a.gameObject.name, b.gameObject.name));
        for (int i = 0; i < 41; i++)
        {
            slotUi[i] = slots[i];
        }
        itemCount = new GameObject[38];
        for (int i = 0; i < 30; i++)
        {
            itemCount[i] = inventoryUI.GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponentInChildren<Transform>(true).gameObject;
        }
        for (int i = 30; i < 38; i++)
        {
            itemCount[i] = playerUI.GetChild(1).GetChild(i - 30).GetChild(2).GetComponentInChildren<Transform>(true).gameObject;
        }
        itemQuantity = new Text[38];
        for (int i = 0; i < 30; i++)
        {
            itemQuantity[i] = inventoryUI.GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetChild(0).GetComponentInChildren<Text>(true);
        }
        for (int i = 30; i < 38; i++)
        {
            itemQuantity[i] = playerUI.GetChild(1).GetChild(i - 30).GetChild(2).GetChild(0).GetComponentInChildren<Text>(true);
        }
        frameColor = new Sprite[4];
        frameColor[0] = Resources.Load<Sprite>("Sprites/Frame_ItemFrame01_Color_Brown");
        frameColor[1] = Resources.Load<Sprite>("Sprites/Frame_ItemFrame01_Color_Green");
        frameColor[2] = Resources.Load<Sprite>("Sprites/Frame_ItemFrame01_Color_Red");
        frameColor[3] = Resources.Load<Sprite>("Sprites/Frame_ItemFrame01_Color_Yellow");
        #region 아이템 배열
        image_Item = new Sprite[38];
        image_Item[0] = Resources.Load<Sprite>("Sprites/Sand");
        image_Item[1] = Resources.Load<Sprite>("Sprites/Grass");
        image_Item[2] = Resources.Load<Sprite>("Sprites/Snow");
        image_Item[3] = Resources.Load<Sprite>("Sprites/Metal");
        image_Item[4] = Resources.Load<Sprite>("Sprites/Gold");
        image_Item[5] = Resources.Load<Sprite>("Sprites/Coal");
        image_Item[6] = Resources.Load<Sprite>("Sprites/Brown_Brick");
        image_Item[7] = Resources.Load<Sprite>("Sprites/Black_Brick");
        image_Item[8] = Resources.Load<Sprite>("Sprites/Random");
        image_Item[9] = Resources.Load<Sprite>("Sprites/Wood");
        image_Item[10] = Resources.Load<Sprite>("Sprites/Archer T1");
        image_Item[11] = Resources.Load<Sprite>("Sprites/Archer T2");
        image_Item[12] = Resources.Load<Sprite>("Sprites/Warrior T1");
        image_Item[13] = Resources.Load<Sprite>("Sprites/Warrior T2");
        image_Item[14] = Resources.Load<Sprite>("Sprites/Archer Helmet T1");
        image_Item[15] = Resources.Load<Sprite>("Sprites/Archer Helmet T2");
        image_Item[16] = Resources.Load<Sprite>("Sprites/Warrior Helmet T1");
        image_Item[17] = Resources.Load<Sprite>("Sprites/Warrior Helmet T2");
        image_Item[18] = Resources.Load<Sprite>("Sprites/Pickaxe");
        image_Item[19] = Resources.Load<Sprite>("Sprites/Axe");
        image_Item[20] = Resources.Load<Sprite>("Sprites/Sword");
        image_Item[21] = Resources.Load<Sprite>("Sprites/Sword T1");
        image_Item[22] = Resources.Load<Sprite>("Sprites/Sword T2");
        image_Item[23] = Resources.Load<Sprite>("Sprites/Bow");
        image_Item[24] = Resources.Load<Sprite>("Sprites/Bow T1");
        image_Item[26] = Resources.Load<Sprite>("Sprites/Bow T2");
        image_Item[27] = Resources.Load<Sprite>("Sprites/Torch Light");
        image_Item[28] = Resources.Load<Sprite>("Sprites/Cloak");
        image_Item[29] = Resources.Load<Sprite>("Sprites/Potal T1");
        image_Item[30] = Resources.Load<Sprite>("Sprites/Potal T2");
        image_Item[31] = Resources.Load<Sprite>("Sprites/Spider Wap");
        image_Item[32] = Resources.Load<Sprite>("Sprites/Slime Drop");
        image_Item[33] = Resources.Load<Sprite>("Sprites/String");
        image_Item[34] = Resources.Load<Sprite>("Sprites/Bon");
        image_Item[35] = Resources.Load<Sprite>("Sprites/Wood Group");
        image_Item[36] = Resources.Load<Sprite>("Sprites/Hp Potion");
        image_Item[37] = Resources.Load<Sprite>("Sprites/Mp Potion");
        #endregion
        border = new Sprite[2];
        border[0] = Resources.Load<Sprite>("Sprites/Frame_ListFrame02_s");
        border[1] = Resources.Load<Sprite>("Sprites/Frame_ItemFrame03_f");
        
        image_Status = statusUI.GetComponentInChildren<Transform>(true).gameObject;
        playerStat = statusUI.GetChild(0).GetChild(1).GetChild(0).GetComponentInChildren<Text>(true);
        curruntStat = statusUI.GetChild(0).GetChild(2).GetChild(1).GetComponentInChildren<Text>(true);
        statUp = new Button[4];
        statUp[0] = statusUI.GetChild(0).GetChild(2).GetChild(2).GetComponentInChildren<Button>(true);
        statUp[1] = statusUI.GetChild(0).GetChild(2).GetChild(3).GetComponentInChildren<Button>(true);
        statUp[2] = statusUI.GetChild(0).GetChild(2).GetChild(4).GetComponentInChildren<Button>(true);
        statUp[3] = statusUI.GetChild(0).GetChild(2).GetChild(5).GetComponentInChildren<Button>(true);
        statDown = new Button[4];
        statDown[0] = statusUI.GetChild(0).GetChild(2).GetChild(6).GetComponentInChildren<Button>(true);
        statDown[1] = statusUI.GetChild(0).GetChild(2).GetChild(7).GetComponentInChildren<Button>(true);
        statDown[2] = statusUI.GetChild(0).GetChild(2).GetChild(8).GetComponentInChildren<Button>(true);
        statDown[3] = statusUI.GetChild(0).GetChild(2).GetChild(9).GetComponentInChildren<Button>(true);
        skillImage = new Image[2];
        skillImage[0] = statusUI.GetChild(0).GetChild(3).GetChild(0).GetComponentInChildren<Image>(true);
        skillImage[1] = statusUI.GetChild(0).GetChild(4).GetChild(0).GetComponentInChildren<Image>(true);
        skillLevel = new Text[2];
        skillLevel[0] = statusUI.GetChild(0).GetChild(3).GetChild(0).GetChild(2).GetComponentInChildren<Text>(true);
        skillLevel[1] = statusUI.GetChild(0).GetChild(4).GetChild(0).GetChild(2).GetComponentInChildren<Text>(true);
        skillTooltip = new Text[2];
        skillTooltip[0] = statusUI.GetChild(0).GetChild(3).GetChild(1).GetChild(0).GetComponentInChildren<Text>(true);
        skillTooltip[1] = statusUI.GetChild(0).GetChild(4).GetChild(1).GetChild(0).GetComponentInChildren<Text>(true);
        skillUp = new Button[2];
        skillUp[0] = statusUI.GetChild(0).GetChild(3).GetChild(0).GetChild(0).GetComponentInChildren<Button>(true);
        skillUp[1] = statusUI.GetChild(0).GetChild(4).GetChild(0).GetChild(0).GetComponentInChildren<Button>(true);
        skillDown = new Button[2];
        skillDown[0] = statusUI.GetChild(0).GetChild(3).GetChild(0).GetChild(1).GetComponentInChildren<Button>(true);
        skillDown[1] = statusUI.GetChild(0).GetChild(4).GetChild(0).GetChild(1).GetComponentInChildren<Button>(true);
        skillPoint = statusUI.GetChild(0).GetChild(5).GetChild(0).GetComponentInChildren<Text>(true);

        image_Tooltip = tooltipUI.GetComponentInChildren<Transform>(true).gameObject;
        tooltip = tooltipUI.GetChild(0).GetComponentInChildren<Text>(true);

        currentSlot = DragSlotUI.GetComponentInChildren<Transform>(true).gameObject;
        currentImage = DragSlotUI.GetChild(0).GetComponentInChildren<Image>(true);
        text_currentSlot = DragSlotUI.GetChild(1).GetComponentInChildren<Transform>(true).gameObject;
        SlotItemCount = DragSlotUI.GetChild(1).GetChild(0).GetComponentInChildren<Text>(true);
        #endregion
    }

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
        player = GameObject.Find("Player").GetComponent<PlayerControl>();

        menuImage.SetActive(false);
        settingMenu.SetActive(false);
        resolutionWindow.SetActive(false);
        image_Status.SetActive(false);
        inventory.SetActive(false);
        playerView.SetActive(false);
        image_Tooltip.SetActive(false);
        SetCursorState(true);
    }

    private void LateUpdate()
    {
        MenuOnOff();
        StatusOnOff();
        InventoryOnOff();
        InventoryCheck();
        SetQuickSlot();
        TimerCheck();
    }

    #region  세팅 ui 설정
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
    #endregion

    #region 윈도우 onoff 메소드
    public void MenuOnOff()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuImage.activeSelf)
            {
                isQuickSlot = false;
                target.SetActive(false);
                SetCursorState(false);
                Time.timeScale = 0;
                menuImage.SetActive(true);
                image_Status.SetActive(false);
                inventory.SetActive(false);
                playerView.SetActive(false);
            }
            else
            {
                isQuickSlot = true;
                target.SetActive(true);
                image_Tooltip.SetActive(false);
                SetCursorState(true);
                Time.timeScale = 1;
                menuImage.SetActive(false);
            }
        }
    }

    public void StatusOnOff()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (!image_Status.activeSelf)
            {
                isQuickSlot = false;
                target.SetActive(false);
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
                isQuickSlot = true;
                target.SetActive(true);
                image_Tooltip.SetActive(false);
                SetCursorState(true);
                image_Status.SetActive(false);
                playerView.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }

    private void InventoryOnOff()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!inventory.activeSelf)
            {
                isQuickSlot = false;
                target.SetActive(false);
                SetCursorState(false);
                Time.timeScale = 0;
                inventory.SetActive(true);
                image_Status.SetActive(false);
                menuImage.SetActive(false);
                playerView.SetActive(true);
            }
            else
            {
                isQuickSlot = true;
                target.SetActive(true);
                image_Tooltip.SetActive(false);
                SetCursorState(true);
                Time.timeScale = 1;
                inventory.SetActive(false);
                playerView.SetActive(false);
            }
        }

        StatusCheck();
    }
    #endregion

    public void OnSkillStatusCall()
    {
        PlayerData playerData = player.playerData;

        if (playerData.job.Equals("전사"))
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

    #region 스테이터스 텍스트 
    public void MouseEnter(int value)
    {
        PlayerData playerData = player.playerData;

        if (playerData.inventory[value].hasItem)
        {
            int num = 0;
            int tag = playerData.inventory[value].tag;

            image_Tooltip.SetActive(true);
            image_Tooltip.transform.position = itemSlot[value].transform.position + (new Vector3(300, -50, 0));

            for (int i = 0; i < itemInfo.list_AllItem.Count; i++)
            {
                if (tag == itemInfo.list_AllItem[i].tag)
                {
                    num = i;
                    break;
                }
            }

            if (itemInfo.list_AllItem[num].type.Equals("Block"))
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
                                "체력 : + " + itemInfo.list_Armor[i].hp + "\n" +
                                "이동 속도 : + " + itemInfo.list_Armor[i].moveSpeed + "\n" +
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
                                "방어력 : + " + itemInfo.list_Helmet[i].defens + "\n" +
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
                                "공격력 : + " + itemInfo.list_Arms[i].attack + "\n" +
                                "공격 속도 : + " + itemInfo.list_Arms[i].attackSpeed + "\n" +
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
                                "회복량 : + " + itemInfo.list_About[i].recovery + "\n" +
                                "-------------\n" +
                                "" + itemInfo.list_About[i].tooltip;
                        break;
                    }
                }
            }
        }
    }

    private void SkillCheck()
    {
        PlayerData playerData = player.playerData;

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
        PlayerData playerData = player.playerData;

        int count = playerData.playerLevel * 5 - playerData.status.statusPoint;

        if (count > 0)
        {
            for (int i = 0; i < statDown.Length; i++)
            {
                statDown[i].interactable = true;
            }

            if (playerData.status.maxHp <= 100)
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

        if (playerData.status.statusPoint > 0)
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
                            "직업 : " + playerData.job + "\n" +
                            "이름 : " + playerData.playerName + "\n" +
                            "레벨 : " + playerData.playerLevel + "\n" +
                            "체력 : " + (playerData.status.maxHp + player.equip_HP) + " (" + playerData.status.maxHp + " + " + player.equip_HP + ")\n" +
                            "마나 : " + playerData.status.maxMp + "\n" +
                            "공격력 : " + (playerData.status.attack + player.equip_Attack) + " (" + playerData.status.attack + " + " + player.equip_Attack + ")\n" +
                            "방어력 : " + (playerData.status.defens + player.equip_Defense) + " (" + playerData.status.defens + " + " + player.equip_Defense + ")\n" +
                            "이동 속도 : " + (playerData.status.moveSpeed + player.equip_Speed) + " (" + playerData.status.moveSpeed + " + " + player.equip_Speed + ")\n" +
                            "공격 속도 : " + (playerData.status.attackSpeed + player.equip_AttackRate) + " (" + playerData.status.attackSpeed + " + " + player.equip_AttackRate + ")";

        curruntStat.text = " " + playerData.status.maxHp + "\n" +
                            " " + playerData.status.maxMp + "\n" +
                            " " + playerData.status.attack + "\n" +
                            " " + playerData.status.defens + "\n" +
                            " " + playerData.status.statusPoint;

        invenStatus.text =  "직업 : " + playerData.job + "\n" +
                            "체력 : " + (playerData.status.maxHp + player.equip_HP) + " (" + playerData.status.maxHp + " + " + player.equip_HP + ")\n" +
                            "마나 : " + playerData.status.maxMp + "\n" +
                            "공격력 : " + (playerData.status.attack + player.equip_Attack) + " (" + playerData.status.attack + " + " + player.equip_Attack + ")\n" +
                            "방어력 : " + (playerData.status.defens + player.equip_Defense) + " (" + playerData.status.defens + " + " + player.equip_Defense + ")\n" +
                            "이동 속도 : " + (playerData.status.moveSpeed + player.equip_Speed) + " (" + playerData.status.moveSpeed + " + " + player.equip_Speed + ")\n" +
                            "공격 속도 : " + (playerData.status.attackSpeed + player.equip_AttackRate) + " (" + playerData.status.attackSpeed + " + " + player.equip_AttackRate + ")";

        CurrentStatus();
        HpCheck(playerData.status.maxHp + player.equip_HP, playerData.status.currentHp);
    }
    #endregion

    #region  버튼 클릭 메소드
    public void OneSkillUpDown(int value)
    {
        PlayerData playerData = player.playerData;

        playerData.skill[0].skillLevel += value;

        if (playerData.skill[0].skillLevel > 0)
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
        PlayerData playerData = player.playerData;

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
        PlayerData playerData = player.playerData;

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
        HpCheck(playerData.status.maxHp, playerData.status.currentHp);
    }

    public void MpUpDown(int value)
    {
        PlayerData playerData = player.playerData;

        playerData.status.maxMp += value;

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

    public void AttackUpDown(int value)
    {
        PlayerData playerData = player.playerData;

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
        PlayerData playerData = player.playerData;

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
    #endregion

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
        PlayerData playerData = player.playerData;

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

                        if (itemInfo.list_AllItem[k].maxQuantity > 2)
                        {
                            itemCount[i].SetActive(true);
                            itemQuantity[i].text = "" + playerData.inventory[i].quantity;
                        }
                        else if (i < 38)
                        {
                            itemCount[i].SetActive(false);
                        }
                        else
                        {
                            break;
                        }

                        if (itemInfo.list_AllItem[k].type.Equals("Helmet") || itemInfo.list_AllItem[k].type.Equals("Armor") || itemInfo.list_AllItem[k].type.Equals("Arms"))
                        {
                            itemFrame[i].sprite = frameColor[2];
                        }
                        else if (itemInfo.list_AllItem[k].type.Equals("About"))
                        {
                            itemFrame[i].sprite = frameColor[1];
                        }
                        else if (itemInfo.list_AllItem[k].type.Equals("Block"))
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
                    itemSlot[i].sprite = null;
                    itemFrame[i].sprite = frameColor[0];
                }
            }
        }

        for (int i = 0; i < slotUi.Length; i++)
        {
            slotUi[i].quantity = playerData.inventory[i].quantity;
            slotUi[i].tag = playerData.inventory[i].tag;
            slotUi[i].type = playerData.inventory[i].type;
            slotUi[i].hasItem = playerData.inventory[i].hasItem;
            slotUi[i].slotNumber = i;
        }
    }

    public void SetQuickSlot()
    {
        #region 퀵슬롯 슬롯 번호 배정
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerHand = 30;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerHand = 31;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            playerHand = 32;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            playerHand = 33;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            playerHand = 34;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            playerHand = 35;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            playerHand = 36;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            playerHand = 37;
        }

        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (wheelInput > 0)
        {
            playerHand--;
            if (playerHand < 30)
            {
                playerHand = 37;
            }
        }
        else if (wheelInput < 0)
        {
            playerHand++;
            if (playerHand > 37)
            {
                playerHand = 30;
            }
        }

        for (int i = 0; i < 8; i++)
        {
            itemBorder[30 + i].sprite = border[0];
        }
        itemBorder[playerHand].sprite = border[1];
        #endregion

        SetArms();
    }

    #region 플레이어 UI 최신화
    public void HpCheck(int maxHp, int currentHp)
    {
        float goals = currentHp / (float)maxHp;

        if (hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine); // 기존 코루틴 종료
        }

        hpCoroutine = StartCoroutine(HpMpDelay_co(goals));
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
        hpCoroutine = null;
    }

    public void ExpCheck(float maxExp, float currentExp)
    {
        float goals = currentExp / maxExp;

        if (expCoroutine != null)
        {
            StopCoroutine(expCoroutine); // 기존 코루틴 종료
        }

        expCoroutine = StartCoroutine(ExpMpDelay_co(goals));
        expCheck.text = currentExp.ToString("N2") + " / " + maxExp.ToString("N2") + "( " + (goals * 100) + "% )";
        string level = string.Format("{0:D2}", player.playerData.playerLevel);
        player_Level.text = level;
    }

    IEnumerator ExpMpDelay_co(float goals)
    {
        float timer = 0f;
        float current = expBar.value;
        float duration = 1f;

        while (timer <= duration)
        {
            yield return null;
            timer += Time.deltaTime;
            float t = timer / duration;
            expBar.value = Mathf.Lerp(current, goals, t);
        }

        expBar.value = goals;
        hpCoroutine = null;
    }
    #endregion

    #region 장비 착용에 따른 스테이터스 변경 메소드
    private int GetHelmetType()
    {
        PlayerData playerData = player.playerData;

        if (playerData.inventory[38].hasItem)
        {
            int tag = playerData.inventory[38].tag;
            for (int i = 0; i < itemInfo.list_Helmet.Count; i++)
            {
                if (tag == itemInfo.list_Helmet[i].tag)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    private int GetArmorType()
    {
        PlayerData playerData = player.playerData;

        if (playerData.inventory[39].hasItem)
        {
            int tag = playerData.inventory[39].tag;
            for (int i = 0; i < itemInfo.list_Armor.Count; i++)
            {
                if (tag == itemInfo.list_Armor[i].tag)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    private int GetQuickItemType()
    {
        PlayerData playerData = player.playerData;

        if (playerData.inventory[playerHand].hasItem)
        {
            int tag = playerData.inventory[playerHand].tag;
            for (int i = 0; i < itemInfo.list_Arms.Count; i++)
            {
                if (tag == itemInfo.list_Arms[i].tag)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    public void SetArmor()
    {
        int armorNumber = GetArmorType();

        if (armorNumber >= 0)
        {
            player.equip_HP = itemInfo.list_Armor[armorNumber].hp;
            player.equip_Speed = itemInfo.list_Armor[armorNumber].moveSpeed;
        }
        else
        {
            player.equip_HP = 0;
            player.equip_Speed = 0;
        }
    }

    public void SetHelmet()
    {
        int helmetNumber = GetHelmetType();

        if (helmetNumber >= 0)
        {
            player.equip_Defense = itemInfo.list_Helmet[helmetNumber].defens;
        }
        else
        {
            player.equip_Defense = 0;
        }
    }

    public void SetArms()
    {
        int armsNumber = GetQuickItemType();

        if (armsNumber >= 0)
        {
            player.equip_Attack = itemInfo.list_Arms[armsNumber].attack;
            player.equip_AttackRate = itemInfo.list_Arms[armsNumber].attackSpeed;
        }
        else
        {
            player.equip_Attack = 0;
            player.equip_AttackRate = 0;
        }
    }

    public void CurrentStatus()
    {
        SetArmor();
        SetHelmet();
    }
    #endregion

    #region 인벤토리 아이템 최신화
    public void ChangedItme(int currentSlot, int newSlot)
    {
        if(currentSlot < 38)
        {
            itemCount[currentSlot].SetActive(false);
        }
        if(newSlot < 38)
        {
            itemCount[newSlot].SetActive(false);
        }

        PlayerData playerData = player.playerData;
        if (playerData.inventory[newSlot].hasItem)
        {
            int tag = playerData.inventory[newSlot].tag;
            int quantity = playerData.inventory[newSlot].quantity;
            string type = playerData.inventory[newSlot].type;

            playerData.inventory[newSlot].tag = playerData.inventory[currentSlot].tag;
            playerData.inventory[newSlot].quantity = playerData.inventory[currentSlot].quantity;
            playerData.inventory[newSlot].type = playerData.inventory[currentSlot].type;

            playerData.inventory[currentSlot].tag = tag;
            playerData.inventory[currentSlot].quantity = quantity;
            playerData.inventory[currentSlot].type = type;
        }
        else
        {
            playerData.inventory[newSlot].hasItem = true;
            playerData.inventory[newSlot].tag = playerData.inventory[currentSlot].tag;
            playerData.inventory[newSlot].quantity = playerData.inventory[currentSlot].quantity;
            playerData.inventory[newSlot].type = playerData.inventory[currentSlot].type;

            playerData.inventory[currentSlot].hasItem = false;
            playerData.inventory[currentSlot].tag = 0;
            playerData.inventory[currentSlot].quantity = 0;
            playerData.inventory[currentSlot].type = null;
        }

        InventoryCheck();
        StatusCheck();
    }

    public void AddItem(int tag, string type, int quantity, int currentSlot)
    {
        PlayerData playerData = player.playerData;
        if (type.Equals("Armor") || type.Equals("Cloak") || type.Equals("Helmet") && currentSlot >= 0)
        {
            NewItemSlot(tag, currentSlot);
        }
        else
        {
            for (int i = 0; i < playerData.inventory.Length - 3; i++)
            {
                if (tag == itemInfo.list_AllItem[i].tag)
                {
                    if (itemInfo.list_AllItem[i].maxQuantity >= 5)
                    {
                        CheckInven(tag, quantity, currentSlot);
                        return;
                    }
                }
            }
            NewItemSlot(tag, currentSlot);
        }
    }

    private void CheckInven(int tag, int quantity, int currentSlot)
    {
        PlayerData playerData = player.playerData;

        for (int i = 0; i < playerData.inventory.Length - 3; i++)
        {
            if (playerData.inventory[i].hasItem)
            {
                if (tag == playerData.inventory[i].tag)
                {
                    if (playerData.inventory[i].quantity > 99 - quantity)
                    {
                        continue;
                    }
                    playerData.inventory[i].quantity += quantity;
                    return;
                }
            }
        }
        NewItemSlot(tag, currentSlot);
    }

    private void NewItemSlot(int tag, int currentSlot)
    {
        PlayerData playerData = player.playerData;

        if(currentSlot < 0)
        {
            for (int i = 30; i < 38; i++)
            {
                if (!playerData.inventory[i].hasItem)
                {
                    NewChangedItme(tag, i);
                    return;
                }
            }
            for (int i = 0; i < 30; i++)
            {
                if (!playerData.inventory[i].hasItem)
                {
                    NewChangedItme(tag, i);
                    return;
                }
            }
        }

        for (int i = 30; i < 38; i++)
        {
            if (!playerData.inventory[i].hasItem)
            {
                ChangedItme(currentSlot, i);
                return;
            }
        }
        for (int i = 0; i < 30; i++)
        {
            if (!playerData.inventory[i].hasItem)
            {
                ChangedItme(currentSlot, i);
                return;
            }
        }
    }

    public void NewChangedItme(int tag, int newSlot)
    {
        PlayerData playerData = player.playerData;
        int itemNumber = 0;
        
        for(int i = 0; i < itemInfo.list_AllItem.Count; i++)
        {
            if(tag == itemInfo.list_AllItem[i].tag)
            {
                itemNumber = i;
                break;
            }
        }

        playerData.inventory[newSlot].hasItem = true;
        playerData.inventory[newSlot].tag = itemInfo.list_AllItem[itemNumber].tag;
        playerData.inventory[newSlot].quantity = itemInfo.list_AllItem[itemNumber].quantity;
        playerData.inventory[newSlot].type = itemInfo.list_AllItem[itemNumber].type;

        InventoryCheck();
        StatusCheck();
    }
    #endregion

    #region 아이템 드래그 앤 드롭 메소드
    public void SlotNumberReset(int tag, int quantity, Vector2 pos)
    {
        for(int i = 0; i < itemInfo.list_AllItem.Count; i++)
        {
            if(tag == itemInfo.list_AllItem[i].tag)
            {
                currentSlot.SetActive(true);
                currentImage.sprite = image_Item[i];
                if(itemInfo.list_AllItem[i].maxQuantity > 5)
                {
                    text_currentSlot.SetActive(true);
                    SlotItemCount.text = "" + quantity;
                }
                currentSlot.transform.position = pos;
                break;
            }
        }
    }

    public void DragInItem(Vector2 pos)
    {
        currentSlot.transform.position = pos;
    }

    public void DragDropItem(int slotNumber, GameObject newObject, string type, int tag, int quantity)
    {
        currentSlot.SetActive(false);
        text_currentSlot.SetActive(false);
        SlotItemCount.text = "";

        if (newObject == null)
        {
            print("버리기");
            return;
        }

        Slot slot = newObject.GetComponent<Slot>();

        if (slot == null || slotNumber == slot.slotNumber)
        {
            return;
        }

        if (slot.slotNumber <= 37)
        {
            if (tag == slot.tag && quantity < 99)
            {
                SumItem(slot, slotNumber);
            }
            else
            {
                ChangedItme(slotNumber, slot.slotNumber);
            }
        }
        else if (slot.slotNumber == 38)
        {
            if (type.Equals("Helmet"))
            {
                ChangedItme(slotNumber, slot.slotNumber);
            }
        }
        else if (slot.slotNumber == 39)
        {
            if (type.Equals("Armor"))
            {
                ChangedItme(slotNumber, slot.slotNumber);
            }
        }
        else if (slot.slotNumber == 40)
        {
            if (type.Equals("Cloak"))

            {
                ChangedItme(slotNumber, slot.slotNumber);
            }
        }
    }

    private void SumItem(Slot slot, int currentSlot)
    {
        PlayerData playerData = player.playerData;

        playerData.inventory[slot.slotNumber].hasItem = true;
        playerData.inventory[slot.slotNumber].tag = playerData.inventory[currentSlot].tag;
        playerData.inventory[slot.slotNumber].quantity += playerData.inventory[currentSlot].quantity;
        playerData.inventory[slot.slotNumber].type = slotUi[currentSlot].type;

        if (playerData.inventory[slot.slotNumber].quantity > 99)
        {
            playerData.inventory[currentSlot].quantity = playerData.inventory[slot.slotNumber].quantity - 99;
            playerData.inventory[slot.slotNumber].quantity = 99;
        }
        else
        {
            playerData.inventory[currentSlot].hasItem = false;
            playerData.inventory[currentSlot].tag = 0;
            playerData.inventory[currentSlot].quantity = 0;
            playerData.inventory[currentSlot].type = null;
            itemSlot[currentSlot].sprite = null;
            itemFrame[currentSlot].sprite = frameColor[0];
            itemCount[currentSlot].SetActive(false);
        }
    }

    public void dragAllocation(GameObject newObject, int tag, int quantity, int slotNumber)
    {
        if (newObject == null)
        {
            return;
        }

        Slot slot = newObject.GetComponent<Slot>();

        if (slot == null || slot.count < 1 || slot.slotNumber >= 38)
        {
            return;
        }

        Inventory inven = player.playerData.inventory[slot.slotNumber];

        slot.count--;

        if (!slot.hasItem)
        {
            for(int i = 0; i < itemInfo.list_AllItem.Count; i++)
            {
                if (tag == itemInfo.list_AllItem[i].tag)
                {
                    if(itemInfo.list_AllItem[i].maxQuantity > 2 && quantity >= 2)
                    {
                        inven.tag = itemInfo.list_AllItem[i].tag;
                        inven.hasItem = true;
                        inven.type = itemInfo.list_AllItem[i].type;
                        inven.quantity = 1;
                        player.playerData.inventory[slotNumber].quantity--;
                        SlotItemCount.text = "" + player.playerData.inventory[slotNumber].quantity;
                    }
                }
            }
        }
        else if (slot.tag == tag)
        {
            if (slot.quantity < 99)
            {
                if (quantity >= 2)
                {
                    inven.quantity++;
                    player.playerData.inventory[slotNumber].quantity--;
                    SlotItemCount.text = "" + player.playerData.inventory[slotNumber].quantity;
                }
            }
        }
    }
    #endregion

    public string GetPlayerHandType()
    {
        Inventory playerData = player.playerData.inventory[playerHand];

        return playerData.type;
    }

    public int GetPlayerHandTag()
    {
        Inventory playerData = player.playerData.inventory[playerHand];

        return playerData.tag;
    }

    private void TimerCheck()
    {
        timer.text = TimeManager.instance.GetInGameTimeString();
    }

    private void DropItem(int slotNumber, int tag, string type, int quantity)
    {
        
    }
}
