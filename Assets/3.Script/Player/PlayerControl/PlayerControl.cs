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
    public enum ItemType { Empty = 0, Sword, Bow, Potion, Block }
    [SerializeField] private ItemType currentItem = ItemType.Empty;

    // --------------- 컴포넌트들 ----------------
    private Animator animator;
    private CharacterController charController;
    [HideInInspector]
    public Input_Info input;
    // ------------------------------------------

    // ---------------------------- 카메라 ------------------------------- 
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject[] virtualCamera = new GameObject[2];

    public GameObject cinemachineCameraTarget_Third; //3인칭타겟
    public GameObject cinemachineCameraTarget_First; //1인칭타겟

    private float topClamp_Third = 70.0f;
    private float bottomClamp_Third = -30.0f;
    private float topClamp_First = 80.0f;
    private float bottomClamp_First = -80.0f;

    private float cinemachineTargetYaw_Third;
    private float cinemachineTargetPitch_Third;
    private float cinemachineTargetPitch_First;

    public float cameraAngleOverride = 0.0f;
    // -------------------------------------------------------------------

    // 플레이어
    public float moveSpeed = 4.0f;
    public float sprintSpeed = 7.3f;
    public float jumpHeight = 2.3f;
    public float gravity = -15.0f;
    public float speedChangeRate = 10.0f; //가속,감속
    public float rotationSmoothTime = 0.12f;

    public bool grounded; //플레이어가 땅을 밟고 있는지
    public float groundedRadius = 0.4f;
    public float groundedOffset = -0.3f;
    public float jumpCool = 0.3f; //착지하고 다시 점프가능하기 까지의 시간
    public float fallTime = 0.15f; //떨어지는 상태로 가기까지의 시간 

    public LayerMask LayerMask_Ground;

    private float speed;
    private float blend_MoveSpeed;
    private float targetRotation = 0.0f;
    private float rotationVelocity;

    private float verticalVelocity;
    private float terminalVelocity = 53.0f;
    private float jumpCoolDelta;
    private float fallTimeDelta;
   
    private float rotationSpeed = 1.0f;
    private const float threshold = 0.01f;

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
    private int animID_AttackSpeed;
    #endregion

    #region 장비스탯변수
    public int equip_HP;
    public int equip_Defense;
    public float equip_Speed;
    public int equip_Attack;
    public float equip_AttackRate;
    #endregion



    //public GameObject[] QuickSlotItem;
    [Header("현재 장착중인 아이템")]
    public GameObject equipItem;

    private Transform rayPoint;
    public LayerMask LayerMask_Destroyable;

    public PlayerData playerData;
    public ItemManager itemInfo;
    public SkillManager skillInfo;

    public static PlayerControl instance = null;

    public delegate void WhenPlayerDie();
    public event WhenPlayerDie whenPlayerDie;

    private InGameUIManager uiManager;



    private void Awake()
    {
        #region 싱글톤
        if (instance == null) 
        {
            instance = this; 
        }
        #endregion

        virtualCamera[0] = GameObject.FindGameObjectWithTag("ThirdPersonCamera");
        virtualCamera[1] = GameObject.FindGameObjectWithTag("FirstPersonCamera");
        virtualCamera[1].SetActive(false);
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        rayPoint = GameObject.FindGameObjectWithTag("RayPoint").transform;


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

        cinemachineTargetYaw_Third = cinemachineCameraTarget_Third.transform.rotation.eulerAngles.y;

        animator = transform.GetChild(0).GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
        input = GetComponent<Input_Info>();

        jumpCoolDelta = jumpCool;
        fallTimeDelta = fallTime;

        uiManager.HpCheck(playerData.status.maxHp, playerData.status.currentHp);
        uiManager.ExpCheck((playerData.playerLevel * playerData.playerLevel - playerData.playerLevel) * 5 + 10, playerData.playerExp);

        #region 장비 교체 확인 용
        playerData.inventory[23].hasItem = true;
        playerData.inventory[23].tag = 101;
        playerData.inventory[23].quantity = 1;
        playerData.inventory[23].type = "Armor";

        playerData.inventory[24].hasItem = true;
        playerData.inventory[24].tag = 105;
        playerData.inventory[24].quantity = 1;
        playerData.inventory[24].type = "Helmet";
        #endregion

        //QuickSlotItem = new GameObject[8];
    }
    private void Update()
    {
        hasAnimator = transform.GetChild(0).TryGetComponent(out animator);

        JumpAndGravity();
        GroundCheck();
        Move();
        Attack();
        VeiwChange();
    }
    private void LateUpdate()
    {
        CameraRotation();
    }


    public void VeiwChange()
    {
        if (input.viewChange)
        {
            //[0]:3인칭, [1]:1인칭
            Debug.Log("뷰 전환");
            if (virtualCamera[0].activeSelf == true) //1인칭으로 전환
            {
                currentView = CameraView.FirstPerson;
                virtualCamera[0].SetActive(false);
                virtualCamera[1].SetActive(true);
            }
            else if (virtualCamera[1].activeSelf == true) //3인칭으로 전환
            {
                currentView = CameraView.ThirdPerson;
                virtualCamera[1].SetActive(false);
                virtualCamera[0].SetActive(true);
            }
            input.viewChange = false;
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
    private void CameraRotation()
    {
        switch(currentView)
        {
            case CameraView.ThirdPerson: //3인칭 카메라 조작
                {
                    if (input.look.sqrMagnitude > threshold)
                    {
                        cinemachineTargetYaw_Third += input.look.x;
                        cinemachineTargetPitch_Third += input.look.y;
                    }

                    cinemachineTargetYaw_Third = ClampAngle(cinemachineTargetYaw_Third, float.MinValue, float.MaxValue);
                    cinemachineTargetPitch_Third = ClampAngle(cinemachineTargetPitch_Third, bottomClamp_Third, topClamp_Third);

                    cinemachineCameraTarget_Third.transform.rotation = Quaternion.Euler(cinemachineTargetPitch_Third, cinemachineTargetYaw_Third, 0.0f);
                    break;
                }
            case CameraView.FirstPerson: //1인칭 카메라 조작
                {
                    if (input.look.sqrMagnitude >= threshold)
                    {
                        cinemachineTargetPitch_First += input.look.y * rotationSpeed;
                        rotationVelocity = input.look.x * rotationSpeed;

                        cinemachineTargetPitch_First = ClampAngle(cinemachineTargetPitch_First, bottomClamp_First, topClamp_First);

                        cinemachineCameraTarget_First.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch_First, 0.0f, 0.0f);

                        transform.Rotate(Vector3.up * rotationVelocity);
                    }
                    break;
                }
        }       
    }
    private void Move()
    {
        #region 플레이어 스피드 설정
        float targetSpeed = input.sprint ? sprintSpeed : moveSpeed;
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

        Vector3 inputDirection = new Vector3(input.move.x, 0f, input.move.y).normalized;

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
                        if (currentItem != ItemType.Potion)
                        {
                            transform.rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
                        }
                    }
                    if (input.move == Vector2.zero && input.attack) //움직임X && 공격O
                    {
                        if(currentItem != ItemType.Potion)
                        {
                            transform.rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
                        }                       
                    }
                    //플레이어 움직이기
                    Vector3 targetDirection = Quaternion.Euler(0f, targetRotation, 0f) * Vector3.forward;
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

                // update animator if using character
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
                    CanAction = ActionCool > 1f * 0.5f; //->1f를 staters.attackSpeed 로 바꿔야함

                    if (input.attack && CanAction)
                    {
                        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
                        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

                        if (Physics.Raycast(ray, out RaycastHit hitInfo, 13f, LayerMask_Destroyable)) //설정한 레이어마스크
                        {
                            Debug.Log(hitInfo.transform.name);

                            if (Vector3.Distance(mainCamera.transform.position, rayPoint.position) <
                                Vector3.Distance(mainCamera.transform.position, hitInfo.transform.position))
                            {
                                if (hitInfo.transform.TryGetComponent(out IDestroyable obj))
                                {
                                    obj.TakeDamage(30); // 30 -> playerData.status.attack 후에 변경 
                                }
                                //if (hitInfo.transform.TryGetComponent(out MonsterControl monsterControl))
                                //{
                                //    Vector3 hitPoint = hitInfo.collider.ClosestPoint(transform.position);
                                //    Vector3 hitNormal = transform.position - hitInfo.collider.transform.position;
                                //    monsterControl.OnDamage(20, hitPoint, hitNormal); //20 -> playerData.status.attack 변경필요
                                //}
                            }
                            else //플레이어가 오브젝트에 의해 가려져서 안보이는 경우
                            {
                                Debug.Log("못캐요");
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
            case ItemType.Sword:
                {
                    float equipItem_attackRate = equipItem.GetComponent<Sword>().attackRate;
                    CanAction = ActionCool > 1f * equipItem_attackRate; // 1f -> playerData.status.attackRate 후에 변경
                    if (input.attack && CanAction)
                    {
                        animator.SetFloat(animID_AttackSpeed, 0.533f / equipItem_attackRate);
                        animator.SetTrigger(animID_Swing);
                        equipItem.GetComponent<Sword>().Use();
                        ActionCool = 0;
                    }
                    break;
                }
            case ItemType.Bow:
                {
                    float equipItem_attackRate = equipItem.GetComponent<Bow>().attackRate;
                    CanAction = ActionCool > 1f * equipItem_attackRate; // 1f -> playerData.status.attackRate 후에 변경
                    if (input.attack && CanAction)
                    {
                        animator.SetFloat(animID_AttackSpeed, 0.667f / equipItem_attackRate);
                        animator.SetTrigger(animID_Shot);
                        equipItem.GetComponent<Bow>().Use();
                        ActionCool = 0;
                    }
                    break;
                }
            case ItemType.Potion:
                {
                    CanAction = ActionCool > 1f * 2.33f; //1f를 playerData.status.attackSpeed 로 넣어야함
                    if (input.attack && CanAction)
                    {
                        animator.SetTrigger(animID_Potion);
                        equipItem.GetComponent<Potion>().Use();
                        ActionCool = 0;
                    }
                    break;
                }
            case ItemType.Block:
                {
                    break;
                }
        }
    }
    private void ItemSelect()
    {

    }
    private void Skill_1() //Q 
    {
        
    }
    private void Skill_2() //E
    {

    }
    

    public void OnDamage(int damage, Vector3 hitPosition, Vector3 hitNomal)
    {
        Status status = playerData.status;

        status.currentHp -= damage - Mathf.RoundToInt(damage * Mathf.RoundToInt(100 * status.defens / (status.defens + 50)) * 0.01f);
        if(status.currentHp <= 0)
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
        playerData.playerExp -= (playerData.playerLevel * playerData.playerLevel - playerData.playerLevel) * 5 + 10;
        playerData.playerLevel++;
        uiManager.ExpCheck((playerData.playerLevel * playerData.playerLevel - playerData.playerLevel) * 5 + 10, playerData.playerExp);
    }



    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f)
        {
            lfAngle += 360f;
        }
            
        if (lfAngle > 360f)
        {
            lfAngle -= 360f;
        }
        
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
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


