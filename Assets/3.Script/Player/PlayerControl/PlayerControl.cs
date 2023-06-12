using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Xml.Serialization;

public class PlayerControl : MonoBehaviour, IDamage
{
    //카메라 시점 열거형
    public enum CameraView { ThirdPerson = 0, FirstPerson = 1 };
    public CameraView currentView = CameraView.ThirdPerson;

    //장착아이템 열거형
    public enum ItemType { Empty = 0, Arms, About, Block, Structure}
    [SerializeField] private ItemType currentItem = ItemType.Empty;

    [Header("현재 장착중인 아이템")]
    public GameObject equipItem;
    [Header("현재 장착중인 아이템의 태그,타입,수량")]
    public int quickSlotItem_Tag;
    public string quickSlotItem_Type;
    public int quickSlotItem_Quantity;


    // --------------- 컴포넌트들 ----------------
    public Animator animator;
    private CharacterController charController;
    [HideInInspector]
    public Input_Info input;
    // ------------------------------------------

    // ---------------------------- 카메라 ------------------------------- 
    [SerializeField] private GameObject mainCamera;
    public float cameraAngleOverride = 0.0f;
    // -------------------------------------------------------------------

    // 플레이어
    public float moveSpeed = 4.0f;
    public float sprintSpeed = 7.3f;
    public float jumpHeight = 2.5f;
    public float gravity = -30.0f;
    public float speedChangeRate = 10.0f; //가속,감속
    public float rotationSmoothTime = 0.12f;

    public bool grounded; //플레이어가 땅을 밟고 있는지
    public float groundedRadius = 0.4f;
    public float groundedOffset = -0.3f;
    public float jumpCool = 0.3f; //착지하고 다시 점프가능하기 까지의 시간
    public float fallTime = 0.15f; //떨어지는 상태로 가기까지의 시간 

    public LayerMask LayerMask_Ground;
    public LayerMask LayerMask_Dig;
    public LayerMask layerMask_Block;
    public LayerMask layerMask_Torch;

    private Vector3 targetDirection;
    private Vector3 inputDirection;

    private float speed;
    private float blend_MoveSpeed;
    private float targetRotation = 0.0f;
    private float rotationVelocity;

    private float verticalVelocity;
    private float terminalVelocity = 53.0f;
    private float jumpCoolDelta;
    private float fallTimeDelta;

    private float DodgeCoolDelta;
    private float DodgeTimeDelta;
    public bool isDodging;
    public bool canDodge;
    public float DodgePower;
    public float DodgeCool;
    public float DodgeDuration;

    private bool CanAction;
    private float ActionCool = 0;
    private bool isDead = false;

    private bool hasAnimator;

   
    #region 애니메이션 파라미터ID
    private int animID_Speed;
    private int animID_Ground;
    private int animID_Jump;
    private int animID_Falling;
    private int animID_Attack;
    private int animID_Swing;
    private int animID_Shot;
    private int animID_Die;
    private int animID_Potion;
    private int animID_Roll;
    private int animID_A1;
    private int animID_A2;
    private int animID_AttackSpeed;  
    #endregion

    #region 장비스탯변수
    public int equip_HP;
    public int equip_Defense;
    public float equip_Speed;
    public int equip_Attack;
    public float equip_AttackRate;
    #endregion


    private Transform rayPoint;
    

    public PlayerData playerData;
    public ItemManager itemInfo;
    public SkillManager skillInfo;

    public static PlayerControl instance = null;

    public delegate void WhenPlayerDie();
    public event WhenPlayerDie whenPlayerDie;

    private InGameUIManager uiManager;

    public List<GameObject> ItemList_Equip = new List<GameObject>();
    public List<GameObject> ItemList_Armor = new List<GameObject>();
    public List<GameObject> ItemList_Hat = new List<GameObject>();    
    public List<GameObject> ItemList_Cloak = new List<GameObject>();
    public List<GameObject> ItemList_Build = new List<GameObject>();
    public List<GameObject> ItemList_Potion = new List<GameObject>();
    public List<GameObject> ItemList_Structure = new List<GameObject>();

    private Transform head;

    public int playerHand;


    private void Awake()
    {
        #region 싱글톤
        if (instance == null) 
        {
            instance = this; 
        }
        #endregion

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        rayPoint = GameObject.FindGameObjectWithTag("RayPoint").transform;
        head = GameObject.FindGameObjectWithTag("Head").transform;

        uiManager = FindObjectOfType<InGameUIManager>();
        itemInfo = FindObjectOfType<ItemManager>();
        skillInfo = FindObjectOfType<SkillManager>();


        TryGetComponent(out itemInfo);
        TryGetComponent(out skillInfo);

        playerData = DataManager.instance.PlayerDataGet(DataManager.instance.saveNumber);
    }
    private void Start()
    {
        AssignAnimationID();
        hasAnimator = transform.GetChild(0).TryGetComponent(out animator);
        animator = transform.GetChild(0).GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
        input = GetComponent<Input_Info>();

        jumpCoolDelta = jumpCool;
        fallTimeDelta = fallTime;

        uiManager.HpCheck(playerData.status.maxHp, playerData.status.currentHp);
        uiManager.ExpCheck((playerData.playerLevel * playerData.playerLevel - playerData.playerLevel) * 5 + 10, playerData.playerExp);
        uiManager.MpCheck(playerData.status.maxMp, playerData.status.currentMp, 0, 0);

        CustomizePlayer();
    }
    private void Update()
    {
        GroundCheck();
        Move();
        JumpAndGravity();
        DodgeRoll();
        Attack();
        Build();
        QuickSlotItem();
        ArmorSelect();
        if (input.skill_1)
        {
            Skill_1();
            input.skill_1 = false;
        }
        if (input.skill_2)
        {
            Skill_2();
            input.skill_2 = false;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            playerData.status.currentHp += 50;
        }
    }






    private void GroundCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);

        grounded = Physics.CheckSphere(spherePosition, groundedRadius, LayerMask_Ground, QueryTriggerInteraction.Ignore);

        if (hasAnimator)
        {
            animator.SetBool(animID_Ground, grounded);
        }
    }
    private void Move()
    {
        #region 플레이어 스피드 설정
        float targetSpeed = input.sprint ? sprintSpeed : playerData.status.moveSpeed;
        if (input.move == Vector2.zero)
        {
            targetSpeed = 0.0f;
        }

        float curHorizontalSpeed = new Vector3(charController.velocity.x, 0.0f, charController.velocity.z).magnitude;
        float speedOffset = 0.1f;

        //가속,감속
        if (curHorizontalSpeed > targetSpeed + speedOffset || curHorizontalSpeed < targetSpeed - speedOffset)
        {
            speed = Mathf.Lerp(curHorizontalSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        //블렌드 파라미터 -> 움직임속도
        blend_MoveSpeed = Mathf.Lerp(blend_MoveSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
        if (blend_MoveSpeed < 0.1f)
        {
            blend_MoveSpeed = 0.0f;
        }
        #endregion

        inputDirection = new Vector3(input.move.x, 0f, input.move.y).normalized;

        #region 3인칭 && 1인칭 플레이어 방향 설정 및 움직이기
        switch (currentView)
        {
            case CameraView.ThirdPerson: //3인칭
                {               
                    if (input.move != Vector2.zero) //움직임O && 공격X
                    {
                        targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
                        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);
                        transform.rotation = Quaternion.Euler(0f, rotation, 0f);
                    }
                    if (input.move != Vector2.zero && input.attack) //움직임O && 공격O
                    {
                        if (currentItem != ItemType.About)
                        {
                            transform.rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
                        }
                    }
                    if (input.move == Vector2.zero && input.attack) //움직임X && 공격O
                    {
                        if(currentItem != ItemType.About)
                        {
                            transform.rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
                        }                       
                    }
                    //플레이어 움직이기
                    targetDirection = Quaternion.Euler(0f, targetRotation, 0f) * Vector3.forward;
                    charController.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                                         new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
                    break;
                }
            case CameraView.FirstPerson: //1인칭
                {
                    if (input.move != Vector2.zero)
                    {
                        inputDirection = transform.right * input.move.x + transform.forward * input.move.y;
                    }
                    //플레이어 움직이기
                    charController.Move(inputDirection.normalized * (speed * Time.deltaTime) +
                        new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
                    break;
                }                
        }          
        #endregion
        
        //움직임 애니메이션
        if (hasAnimator)
        {
            animator.SetFloat(animID_Speed, blend_MoveSpeed);
        }
    }
    private void DodgeRoll()
    {
        DodgeCoolDelta -= Time.deltaTime;
        canDodge = (DodgeCoolDelta <= 0);

        if (canDodge) // 구르기 가능 상태
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && inputDirection.magnitude != 0 && grounded)
            {
                animator.SetTrigger(animID_Roll);
                DodgeCoolDelta = DodgeCool;
                DodgeTimeDelta = DodgeDuration; 
                StartCoroutine(PerformDodge()); 
            }
        }
    }
    private IEnumerator PerformDodge()
    {
        while (DodgeTimeDelta > 0)
        {
            isDodging = true;
            charController.Move(targetDirection * DodgePower * Time.deltaTime);
            DodgeTimeDelta -= Time.deltaTime;
            yield return null;
        }
        isDodging = false;
    }
    private void JumpAndGravity()
    {
        if (grounded)
        {
            fallTimeDelta = fallTime;

            if (hasAnimator)
            {
                animator.SetBool(animID_Jump, false);
                animator.SetBool(animID_Falling, false);
            }


            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }

            // Jump
            if (input.jump && jumpCoolDelta <= 0.0f)
            {

                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                if (hasAnimator)
                {
                    animator.SetBool(animID_Jump, true);
                }
            }

            if (jumpCoolDelta >= 0.0f)
            {
                jumpCoolDelta -= Time.deltaTime;
            }
        }
        else
        {
            jumpCoolDelta = jumpCool;

            if (fallTimeDelta >= 0.0f)
            {
                fallTimeDelta -= Time.deltaTime;
            }
            else
            {
                if (hasAnimator)
                {
                    animator.SetBool(animID_Falling, true);
                }
            }

            input.jump = false;
        }
 
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }
    private void Attack() //마우스좌클릭 
    {
        ActionCool += Time.deltaTime;

        switch (currentItem) //장착중인 현재아이템
        {
            case ItemType.Empty:
                {
                    CanAction = ActionCool > 1f * 0.5f; 

                    if (input.attack && CanAction)
                    {
                        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
                        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

                        if (Physics.Raycast(ray, out RaycastHit hitInfo, 15f, LayerMask_Dig))
                        {
                            if (hitInfo.transform.TryGetComponent(out IDestroyable obj))
                            {
                                obj.TakeDamage(5);
                            }
                        }
                        else
                        {
                        }
                        animator.SetTrigger(animID_Attack);
                        ActionCool = 0;
                    }
                    break;
                }
            case ItemType.Arms:
                {
                    float equipItem_attackRate;

                    if (equipItem.TryGetComponent(out Sword sword))
                    {
                        equipItem_attackRate = sword.attackRate;
                        CanAction = ActionCool > 1f * equipItem_attackRate; // 1f -> playerData.status.attackRate 후에 변경
                        if (input.attack && CanAction)
                        {
                            AudioManager.instance.PlaySFX("PlayerSwordAttack");
                            animator.SetFloat(animID_AttackSpeed, 0.533f / equipItem_attackRate);
                            animator.SetTrigger(animID_Swing);
                            equipItem.GetComponent<Sword>().Use();
                            ActionCool = 0;
                        }
                    }
                    else if (equipItem.TryGetComponent(out Bow bow))
                    {
                        equipItem_attackRate = bow.attackRate;
                        CanAction = ActionCool > 1f * equipItem_attackRate; // 1f -> playerData.status.attackRate 후에 변경
                        if (input.attack && CanAction)
                        {
                            AudioManager.instance.PlaySFX("PlayerBowAttack");
                            animator.SetFloat(animID_AttackSpeed, 0.667f / equipItem_attackRate);
                            animator.SetTrigger(animID_Shot);
                            equipItem.GetComponent<Bow>().Use();
                            ActionCool = 0;
                        }
                    }
                    else if (equipItem.TryGetComponent(out Pickaxe pickaxe))
                    {
                        equipItem_attackRate = 1f * pickaxe.attackRate; //0.5f를 pickaxe.attackRate 로 변경 
                        CanAction = ActionCool > 1f * equipItem_attackRate; //0.5f 를 equipItem.attackRate 로 변경
                        if (input.attack && CanAction)
                        {
                            Vector2 centerPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
                            Ray ray = Camera.main.ScreenPointToRay(centerPoint);
                            if (Physics.Raycast(ray, out RaycastHit hitInfo, 15f, LayerMask_Dig))
                            {
                                if (hitInfo.transform.TryGetComponent(out IDestroyable obj))
                                {
                                    obj.TakeDamage(pickaxe.attackDamage);
                                }
                            }
                            else
                            {
                                Debug.Log("헛손질");
                            }
                            ActionCool = 0;
                        }
                    }
                    else if (equipItem.TryGetComponent(out Axe axe))
                    {
                        equipItem_attackRate = 1f * axe.attackRate; //0.5f를 axe.attackRate 로 변경 
                        CanAction = ActionCool > 1f * equipItem_attackRate; //0.5f 를 equipItem.attackRate 로 변경
                        if (input.attack && CanAction)
                        {
                            ActionCool = 0;
                        }
                    }
                    else if (equipItem == ItemList_Equip[8])
                    {
                        CanAction = ActionCool > 1f * 0.5f;

                        if (input.attack && CanAction)
                        {
                            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
                            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

                            if (Physics.Raycast(ray, out RaycastHit hitInfo, 15f, LayerMask_Dig))
                            {
                                if (hitInfo.transform.TryGetComponent(out IDestroyable obj))
                                {
                                    obj.TakeDamage(5);
                                }
                            }
                            else
                            {
                                Debug.Log("헛손질");
                            }
                            animator.SetTrigger(animID_Attack);
                            ActionCool = 0;
                        }
                    }
                    break;
                }                   
            case ItemType.About:
                {
                    CanAction = ActionCool > 1f * 2.33f; //1f를 playerData.status.attackSpeed 로 넣어야함
                    if (input.attack && CanAction)
                    {
                        AudioManager.instance.PlaySFX("PlayerUsePotion");
                        animator.SetTrigger(animID_Potion);
                        equipItem.GetComponent<Potion>().Use(playerHand, playerData.inventory[playerHand].tag);
                        ActionCool = 0;
                    }
                    break;
                }
            case ItemType.Block:
                {
                    CanAction = ActionCool > 1f * 0.5f;
                    if (input.attack && CanAction)
                    {
                        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
                        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

                        if (Physics.Raycast(ray, out RaycastHit hitInfo, 15f, LayerMask_Dig))
                        {
                            if (hitInfo.transform.TryGetComponent(out IDestroyable obj))
                            {
                                obj.TakeDamage(5); 
                            }
                        }
                        else
                        {
                            Debug.Log("헛손질");
                        }
                        animator.SetTrigger(animID_Attack);
                        ActionCool = 0;
                    }
                    break;
                }
        }
    }
    private void Build() //마우스우클릭
    {
        if (input.mouse_1)
        {
            switch (currentItem) //장착중인 현재아이템
            {
                case ItemType.Block:
                    {
                        AudioManager.instance.PlaySFX("PlayerBlockInstall");
                        animator.SetTrigger(animID_Attack);
                        CreateBlock();
                        input.mouse_1 = false;                    
                        break;
                    }
                case ItemType.Structure:
                    {
                        AudioManager.instance.PlaySFX("PlayerBlockInstall");
                        animator.SetTrigger(animID_Attack);
                        CreateStructure();
                        input.mouse_1 = false;                      
                        break;
                    }
                case ItemType.Arms:
                    {
                        if (equipItem == ItemList_Equip[8])
                        {
                            AudioManager.instance.PlaySFX("PlayerBlockInstall");
                            animator.SetTrigger(animID_Attack);
                            CreateTorch();
                            input.mouse_1 = false;                            
                        }
                        break;
                    }
                case ItemType.Empty:
                    {
                        break;
                    }
                case ItemType.About:
                    {
                        break;
                    }
            }
        }    
    }
    private void Skill_1() //Q 
    {
        if (playerData.skill[0].hasSkill)
        {
            if (currentItem == ItemType.Arms)
            {
                if (equipItem.TryGetComponent(out Bow bow))
                {
                    bow.Skill_1();
                }
                else if (equipItem.TryGetComponent(out Sword sword))
                {
                    sword.Skill_1();
                }
            }
        }      
    }
    private void Skill_2() //E
    {
        if (playerData.skill[1].hasSkill)
        {
            if (currentItem == ItemType.Arms)
            {
                if (equipItem.TryGetComponent(out Bow bow))
                {
                    bow.Skill_2();
                }
                else if (equipItem.TryGetComponent(out Sword sword))
                {
                    sword.Skill_2();
                }
            }           
        }       
    }





    private void CustomizePlayer()
    {
        head.GetChild(0).GetChild(playerData.hair).gameObject.SetActive(true);
        head.GetChild(1).GetChild(playerData.eye).gameObject.SetActive(true);
        head.GetChild(2).GetChild(playerData.mouth).gameObject.SetActive(true);
        head.GetChild(3).GetChild(playerData.mustache).gameObject.SetActive(true);
        transform.GetChild(0).GetChild(playerData.body).gameObject.SetActive(true);
    }
    public void CreateTorch()
    {
        uiManager.CraftingBlock(playerHand);

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 20f, layerMask_Block))
        {
            //Debug.Log("해당오브젝트의 좌표는" + hitInfo.transform.position);
            //Debug.Log("레이좌표는" + hitInfo.point);
            Vector3 vecDir = hitInfo.point - hitInfo.transform.position;
            //Debug.Log("두 벡터간의 차이는" + vecDir);
            float xValue = vecDir.x;
            float yValue = vecDir.y;
            float zValue = vecDir.z;

            float maxValue = Mathf.Max(Mathf.Abs(xValue), Mathf.Abs(yValue), Mathf.Abs(zValue));
            if (Mathf.Abs(xValue) == maxValue)
            {
                //Debug.Log("vecDir.x가 가장 큰값");
                if (xValue > 0)
                {
                    Instantiate(equipItem, hitInfo.transform.position + Vector3.right * 0.7f, Quaternion.Euler(10f,0,-10f));
                }
                else if (xValue < 0)
                {
                    Instantiate(equipItem, hitInfo.transform.position + Vector3.left * 0.7f, Quaternion.Euler(-10f, 0, 10f));
                }
            }
            else if (Mathf.Abs(yValue) == maxValue)
            {
                //Debug.Log("vecDir.y가 가장 큰값");
                if (yValue > 0)
                {
                    Instantiate(equipItem, hitInfo.transform.position + Vector3.up * 0.8f, Quaternion.identity);
                }
                else if (yValue < 0)
                {
                    Instantiate(equipItem, hitInfo.transform.position + Vector3.down * 0.8f, Quaternion.Euler(0, 0, 180f));
                }
            }
            else if (Mathf.Abs(zValue) == maxValue)
            {
                //Debug.Log("vecDir.z가 가장 큰값");
                if (zValue > 0)
                {
                    Instantiate(equipItem, hitInfo.transform.position + Vector3.forward * 0.7f, Quaternion.Euler(10f, 0, 10f));
                }
                else if (zValue < 0)
                {
                    Instantiate(equipItem, hitInfo.transform.position + Vector3.back * 0.7f, Quaternion.Euler(-10f, 0, -10f));
                }
            }
        }
    }
    public void CreateBlock()
    {
        uiManager.CraftingBlock(playerHand);

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 20f, layerMask_Block))
        {
            //Debug.Log("해당오브젝트의 좌표는" + hitInfo.transform.position);
            //Debug.Log("레이좌표는" + hitInfo.point);
            Vector3 vecDir = hitInfo.point - hitInfo.transform.position;
            //Debug.Log("두 벡터간의 차이는" + vecDir);
            float xValue = vecDir.x;
            float yValue = vecDir.y;
            float zValue = vecDir.z;

            float maxValue = Mathf.Max(Mathf.Abs(xValue), Mathf.Abs(yValue), Mathf.Abs(zValue));

            bool isTorch; //해당 지점에 토치가 설치되어있는지
            Vector3 spherePosition; //토치를 감지할 구

            if (Mathf.Abs(xValue) == maxValue)
            {
                //Debug.Log("vecDir.x가 가장 큰값");
                if (xValue > 0)
                {
                    spherePosition = hitInfo.transform.position + Vector3.right;
                    isTorch = Physics.CheckSphere(spherePosition, groundedRadius, layerMask_Torch, QueryTriggerInteraction.Ignore);
                    if (!isTorch)
                    {
                        Instantiate(equipItem, hitInfo.transform.position + Vector3.right, Quaternion.identity);
                    }
                }
                else if (xValue < 0)
                {
                    spherePosition = hitInfo.transform.position + Vector3.left;
                    isTorch = Physics.CheckSphere(spherePosition, groundedRadius, layerMask_Torch, QueryTriggerInteraction.Ignore);
                    if (!isTorch)
                    {
                        Instantiate(equipItem, hitInfo.transform.position + Vector3.left, Quaternion.identity);
                    }                   
                }
            }
            else if (Mathf.Abs(yValue) == maxValue)
            {
                //Debug.Log("vecDir.y가 가장 큰값");
                if (yValue > 0)
                {
                    spherePosition = hitInfo.transform.position + Vector3.up;
                    isTorch = Physics.CheckSphere(spherePosition, groundedRadius, layerMask_Torch, QueryTriggerInteraction.Ignore);
                    if (!isTorch)
                    {
                        Instantiate(equipItem, hitInfo.transform.position + Vector3.up, Quaternion.identity);
                    }
                }
                else if (yValue < 0)
                {
                    spherePosition = hitInfo.transform.position + Vector3.down;
                    isTorch = Physics.CheckSphere(spherePosition, groundedRadius, layerMask_Torch, QueryTriggerInteraction.Ignore);
                    if (!isTorch)
                    {
                        Instantiate(equipItem, hitInfo.transform.position + Vector3.down, Quaternion.identity);
                    }
                }
            }
            else if (Mathf.Abs(zValue) == maxValue)
            {
                //Debug.Log("vecDir.z가 가장 큰값");
                if (zValue > 0)
                {
                    spherePosition = hitInfo.transform.position + Vector3.forward;
                    isTorch = Physics.CheckSphere(spherePosition, groundedRadius, layerMask_Torch, QueryTriggerInteraction.Ignore);
                    if (!isTorch)
                    {
                        Instantiate(equipItem, hitInfo.transform.position + Vector3.forward, Quaternion.identity);
                    }
                }
                else if (zValue < 0)
                {
                    spherePosition = hitInfo.transform.position + Vector3.back;
                    isTorch = Physics.CheckSphere(spherePosition, groundedRadius, layerMask_Torch, QueryTriggerInteraction.Ignore);
                    if (!isTorch)
                    {
                        Instantiate(equipItem, hitInfo.transform.position + Vector3.back, Quaternion.identity);
                    }
                }
            }
        }
    }
    public void CreateStructure()
    {
        uiManager.CraftingBlock(playerHand);

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 20f, layerMask_Block))
        {
            //Debug.Log("해당오브젝트의 좌표는" + hitInfo.transform.position);
            //Debug.Log("레이좌표는" + hitInfo.point);
            Vector3 vecDir = hitInfo.point - hitInfo.transform.position;
            //Debug.Log("두 벡터간의 차이는" + vecDir);
            float xValue = vecDir.x;
            float yValue = vecDir.y;
            float zValue = vecDir.z;

            float maxValue = Mathf.Max(Mathf.Abs(xValue), Mathf.Abs(yValue), Mathf.Abs(zValue));

            bool isTorch; //해당 지점에 토치가 설치되어있는지
            Vector3 spherePosition; //토치를 감지할 구

            if (Mathf.Abs(yValue) == maxValue)
            {
                //Debug.Log("vecDir.y가 가장 큰값");
                if (yValue > 0)
                {
                    spherePosition = hitInfo.transform.position + Vector3.up;
                    isTorch = Physics.CheckSphere(spherePosition, groundedRadius, layerMask_Torch, QueryTriggerInteraction.Ignore);
                    if (!isTorch)
                    {
                        Instantiate(equipItem, hitInfo.transform.position + Vector3.up * 0.5f, Quaternion.identity);
                    }
                }
            }            
        }
    }






    private void ItemList_Equip_InActive()
    {
        int tag = playerData.inventory[playerHand].tag;

        for (int i = 0; i < ItemList_Equip.Count; i++)
        {
            ItemList_Equip[i].SetActive(false);
        }
    }
    private void QuickSlotItem()
    {
        int tag = playerData.inventory[playerHand].tag;
        if (tag >= 1 && tag <= 10) //블록들
        {
            ItemList_Equip_InActive();
            currentItem = ItemType.Block;
            equipItem = ItemList_Build[tag - 1];
        }
        else if (tag >= 201 && tag <= 209) //무기
        {
            currentItem = ItemType.Arms;
            equipItem = ItemList_Equip[tag - 201];
            for (int i = 0; i < ItemList_Equip.Count; i++)
            {
                if (i == tag - 201)
                {
                    ItemList_Equip[i].SetActive(true);
                }
                else
                {
                    ItemList_Equip[i].SetActive(false);
                }
            }
        }
        else if (tag == 501 || tag == 502) //포션
        {
            ItemList_Equip_InActive();
            currentItem = ItemType.About;
            equipItem = ItemList_Potion[0];
        }
        else if (tag == 408)
        {
            ItemList_Equip_InActive();
            currentItem = ItemType.Structure;
            equipItem = ItemList_Structure[0];
        }
        else
        {
            ItemList_Equip_InActive();
            currentItem = ItemType.Empty;
            equipItem = null;
        }
    }


    private void HatInActive()
    {
        for (int i = 0; i < ItemList_Hat.Count; i++) 
        {
            ItemList_Hat[i].SetActive(false);
        }      
    }
    private void ArmorInActive()
    {
        for (int i = 0; i < ItemList_Armor.Count; i++)
        {
            ItemList_Armor[i].SetActive(false);
        }
    }
    private void ArmorSelect()
    {
        int helmet_Tag = playerData.inventory[38].tag;
        int armor_Tag = playerData.inventory[39].tag;
        int cloak_Tag = playerData.inventory[40].tag;

        if (armor_Tag >= 101 && armor_Tag <= 104) //아머
        {
            for (int i = 0; i < ItemList_Armor.Count; i++)
            {
                if (i == armor_Tag - 101)
                {
                    ItemList_Armor[i].SetActive(true);
                }
                else
                {
                    ItemList_Armor[i].SetActive(false);
                }
            }
            transform.GetChild(0).GetChild(playerData.body).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false; 
        }
        else
        {
            ArmorInActive();
            transform.GetChild(0).GetChild(playerData.body).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }

        if (helmet_Tag >= 105 && helmet_Tag <= 108) //헬멧
        {
            for (int i = 0; i < ItemList_Hat.Count; i++)
            {
                if (i == helmet_Tag - 105)
                {
                    ItemList_Hat[i].SetActive(true);
                }
                else
                {
                    ItemList_Hat[i].SetActive(false);
                }
            }
            head.GetChild(0).GetChild(playerData.hair).gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            HatInActive();
            head.GetChild(0).GetChild(playerData.hair).gameObject.GetComponent<MeshRenderer>().enabled = true;
        }

        if (cloak_Tag == 301) //망토
        {
            ItemList_Cloak[0].SetActive(true);
        }
        else
        {
            ItemList_Cloak[0].SetActive(false);
        }
    }

    



    public void OnDamage(int damage, Vector3 hitPosition, Vector3 hitNomal)
    {      
        Status status = playerData.status;

        if (!isDodging) //테스트해봐야함
        {
            AudioManager.instance.PlaySFX("PlayerHit");
            status.currentHp -= damage - Mathf.RoundToInt(damage * Mathf.RoundToInt(100 * status.defens / (status.defens + 50)) * 0.01f);
        }       

        if(status.currentHp <= 0) //죽었을때
        {
            isDead = true;
            whenPlayerDie?.Invoke();
        }

        uiManager.HpCheck(status.maxHp, status.currentHp);
    }
    public void Die()
    {
        if (isDead)
        {
            charController.enabled = false;
            animator.SetTrigger(animID_Die);
        }       
    }
    public void GetExp(int exp)
    {
        playerData.playerExp += exp;

        int requiredExp = (playerData.playerLevel * playerData.playerLevel - playerData.playerLevel) * 5 + 10;

        while(playerData.playerExp >= requiredExp)
        {
            LevelUp();
        }
        uiManager.ExpCheck(requiredExp, playerData.playerExp);
    }
    public void LevelUp() //후에 스탯상승 추가 
    {
        AudioManager.instance.PlaySFX("PlayerLevelUp");
        Status status = playerData.status;
        playerData.playerExp -= (playerData.playerLevel * playerData.playerLevel - playerData.playerLevel) * 5 + 10;
        playerData.playerLevel++;
        status.statusPoint += 5;
        status.skillPoint += 3;
        uiManager.ExpCheck((playerData.playerLevel * playerData.playerLevel - playerData.playerLevel) * 5 + 10, playerData.playerExp);
    }







    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
            groundedRadius);
    }
    private void AssignAnimationID()
    {
        animID_Speed = Animator.StringToHash("Speed");
        animID_Ground = Animator.StringToHash("Ground");
        animID_Jump = Animator.StringToHash("Jump");
        animID_Falling = Animator.StringToHash("Falling");
        animID_Attack = Animator.StringToHash("Attack");
        animID_Swing = Animator.StringToHash("Swing");
        animID_Shot = Animator.StringToHash("Shot");
        animID_Die = Animator.StringToHash("Die");
        animID_Potion = Animator.StringToHash("Potion");
        animID_AttackSpeed = Animator.StringToHash("AttackSpeed");
        animID_A1 = Animator.StringToHash("A1");
        animID_A2 = Animator.StringToHash("A2");
        animID_Roll = Animator.StringToHash("Roll");
    }
}






[Serializable]
public class PlayerData  // 플레이어 데이터 관리 클레스
{
    [XmlElement]
    public string job;
    [XmlElement]
    public int hair;
    [XmlElement]
    public int eye;
    [XmlElement]
    public int mouth;
    [XmlElement]
    public int mustache;
    [XmlElement]
    public int body;
    [XmlElement]
    public string playerName;
    [XmlElement]
    public int playerLevel;
    [XmlElement]
    public float playerExp;
    [XmlElement]
    public float gameTime;
    [XmlElement]
    public int mapIndex;
    [XmlElement]
    public Status status;
    [XmlElement]
    public Skill[] skill = new Skill[2];
    [XmlElement]
    public Inventory[] inventory = new Inventory[41];
}

[Serializable]
public class Status  // 플레이어 스텟 관리 클래스
{
    [XmlElement]
    public int maxHp;
    [XmlElement]
    public int maxMp;
    [XmlElement]
    public int currentHp;
    [XmlElement]
    public int currentMp;
    [XmlElement]
    public float moveSpeed;
    [XmlElement]
    public float attackSpeed;
    [XmlElement]
    public int attack;
    [XmlElement]
    public int defens;
    [XmlElement]
    public int statusPoint;
    [XmlElement]
    public int skillPoint;
}

[Serializable]
public class Inventory  // 인벤토리 정보 관리 클레스
{
    [XmlElement]
    public int tag;
    [XmlElement]
    public string type;
    [XmlElement]
    public int quantity;
    [XmlElement]
    public bool hasItem;
}

[Serializable]
public class Skill  // 스킬 정보 관리 클레스
{
    [XmlElement]
    public int skillNum;
    [XmlElement]
    public int skillLevel;
    [XmlElement]
    public bool hasSkill;
}


