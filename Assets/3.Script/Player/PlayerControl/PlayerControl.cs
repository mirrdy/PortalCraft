using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Xml.Serialization;

public class PlayerControl : MonoBehaviour
{
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

    //카메라 관련 
    public GameObject cinemachineCameraTarget;
    public float topClamp = 70.0f;
    public float bottomClamp = -30.0f;
    public float cameraAngleOverride = 0.0f;

    //Player
    private float speed;
    private float blend_MoveSpeed;
    private float targetRotation = 0.0f;
    private float rotationVelocity;

    private float verticalVelocity;
    private float terminalVelocity = 53.0f;
    private float jumpCoolDelta;
    private float fallTimeDelta;

    // cinemachine
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;
    private const float threshold = 0.01f;

    private Animator animator;
    private CharacterController charController;
    private Input_Info input;
    [SerializeField] private GameObject mainCamera;

    private bool hasAnimator;

    //애니메이션 파라미터 ID
    private int animID_Speed;
    private int animID_Ground;
    private int animID_Jump;
    private int animID_Falling;
    private int animID_Attack;
    private int animID_Swing;
    private int animID_Shot;

    private bool can_NormalAttack;
    private bool can_WeaponAttack;
    public float normal_AttackCool;
    public float weapon_AttackCool;  
    public Weapon equipWeapon;

    public GameObject[] QuickSlotItem;
    public GameObject equipItem;

    private Transform rayPoint;
    public LayerMask LayerMask_Destroyable;
    private int normalDamage = 21;

    public PlayerData playerData;
    public ItemManager itemInfo;
    public SkillManager skillInfo;

    public static PlayerControl instance = null;

    private void Awake()
    {
        if (instance == null) 
        {
            instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            {
            if (instance != this)
                Destroy(this.gameObject);
            }             
        }

        TryGetComponent(out itemInfo);
        TryGetComponent(out skillInfo);
        playerData = DataManager.instance.PlayerDataGet(DataManager.instance.saveNumber);

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        rayPoint = GameObject.FindGameObjectWithTag("RayPoint").transform;
    }
    private void Start()
    {
        AssignAnimationID();

        cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;

        animator = transform.GetChild(0).GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
        input = GetComponent<Input_Info>();

        jumpCoolDelta = jumpCool;
        fallTimeDelta = fallTime;

        //QuickSlotItem = new GameObject[8];
    }
    private void Update()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        //Debug.DrawRay()

        hasAnimator = transform.GetChild(0).TryGetComponent(out animator);

        JumpAndGravity();
        GroundCheck();
        Move();
        Attack();
    }
    private void LateUpdate()
    {
        CameraRotation();
    }

    private void VeiwChange()
    {

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
        if (input.look.sqrMagnitude > threshold)
        {
            cinemachineTargetYaw += input.look.x;
            cinemachineTargetPitch += input.look.y;
        }

        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);

        cinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, 0.0f);
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

        #region 플레이어 방향 설정
        Vector3 inputDirection = new Vector3(input.move.x, 0f, input.move.y).normalized;

        if (input.move != Vector2.zero) 
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }
        if (input.move != Vector2.zero && input.attack)
        {
            transform.rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
        }
        #endregion

        //플레이어 움직이기 
        Vector3 targetDirection = Quaternion.Euler(0f, targetRotation, 0f) * Vector3.forward;
        charController.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                             new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

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
        //무기 O
        if (equipWeapon != null)
        {
            weapon_AttackCool += Time.deltaTime;
            can_WeaponAttack = weapon_AttackCool > equipWeapon.rate;

            if (equipWeapon.type == Weapon.Type.Melee && input.attack && can_WeaponAttack) //검
            {               
                transform.rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);               
                animator.SetTrigger(animID_Swing);
                equipWeapon.Use();
                weapon_AttackCool = 0;
            }
            else if (equipWeapon.type == Weapon.Type.Range && input.attack && can_WeaponAttack) //활
            {
                Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

                transform.rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);               
                animator.SetTrigger(animID_Shot);
                equipWeapon.Use();
                weapon_AttackCool = 0;
            }
        }
     
        //무기 X -> 파괴가능한 오브젝트의 HP에 데미지
        else if (equipWeapon == null)
        {
            normal_AttackCool += Time.deltaTime;
            can_NormalAttack = normal_AttackCool > 0.6f;

            if (equipWeapon == null && input.attack && can_NormalAttack)
            {
                Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
                Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

                if (Physics.Raycast(ray, out RaycastHit hitInfo, 13f, LayerMask_Destroyable)) //설정한 레이어마스크
                {
                    Debug.Log(hitInfo.transform.name);

                    if (Vector3.Distance(mainCamera.transform.position, rayPoint.position) <
                        Vector3.Distance(mainCamera.transform.position, hitInfo.transform.position))
                    {
                        //Debug.DrawRay(rayPoint.position, (hitInfo.transform.position - rayPoint.position), Color.blue);
                        //Debug.Log("{0} 에게 {1} 의 데미지", hitInfo.transform.name, normalDamage);
                        if(hitInfo.transform.TryGetComponent(out BlockObject block))
                        {
                            block.TakeDamage(normalDamage);
                        }
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

                transform.rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
                animator.SetTrigger(animID_Attack);
                normal_AttackCool = 0;
            }
        }                      
    }
    private void Skill_1() //Q 
    {
        
    }
    private void Skill_2() //E
    {

    }




    private void ItemSelect()
    {
        
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
    public Staters staters;
    [XmlElement]
    public Skill[] skill = new Skill[2];
    [XmlElement]
    public Inventory[] inventory = new Inventory[40];
}

[Serializable]
public class Staters  // 플레이어 스텟 관리 클래스
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
    public int statersPoint;
    [XmlElement]
    public int skillPoint;
}

[Serializable]
public class Inventory  // 인벤토리 정보 관리 클레스
{
    [XmlElement]
    public int tag;
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


