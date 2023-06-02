using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Xml.Serialization;

public class PlayerControl : MonoBehaviour, IDamage
{
    //ī�޶� ���� ������
    public enum cameraView { ThirdPerson = 0, FirstPerson = 1 };
    private cameraView currentView = cameraView.ThirdPerson;

    //���������� ������
    public enum ItemType { Empty = 0, Sword, Bow, Potion, Block }
    [SerializeField] private ItemType currentItem = ItemType.Empty;

    // --------------- ������Ʈ�� ----------------
    private Animator animator;
    private CharacterController charController;
    private Input_Info input;
    // ------------------------------------------

    // ---------------------------- ī�޶� ------------------------------- 
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject[] virtualCamera = new GameObject[2];

    public GameObject cinemachineCameraTarget_Third; //3��ĪŸ��
    public GameObject cinemachineCameraTarget_First; //1��ĪŸ��

    private float topClamp_Third = 70.0f;
    private float bottomClamp_Third = -30.0f;
    private float topClamp_First = 80.0f;
    private float bottomClamp_First = -80.0f;

    private float cinemachineTargetYaw_Third;
    private float cinemachineTargetPitch_Third;
    private float cinemachineTargetPitch_First;

    public float cameraAngleOverride = 0.0f;
    // -------------------------------------------------------------------

    // �÷��̾�
    public float moveSpeed = 4.0f;
    public float sprintSpeed = 7.3f;
    public float jumpHeight = 2.3f;
    public float gravity = -15.0f;
    public float speedChangeRate = 10.0f; //����,����
    public float rotationSmoothTime = 0.12f;

    public bool grounded; //�÷��̾ ���� ��� �ִ���
    public float groundedRadius = 0.4f;
    public float groundedOffset = -0.3f;
    public float jumpCool = 0.3f; //�����ϰ� �ٽ� ���������ϱ� ������ �ð�
    public float fallTime = 0.15f; //�������� ���·� ��������� �ð� 

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

    private bool canAttack;
    private float attackCool = 0;

    private bool hasAnimator;

    #region �ִϸ��̼� �Ķ����ID
    private int animID_Speed;
    private int animID_Ground;
    private int animID_Jump;
    private int animID_Falling;
    private int animID_Attack;
    private int animID_Swing;
    private int animID_Shot;
    #endregion

   

    //public GameObject[] QuickSlotItem;
    [Header("���� �������� ������")]
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
        #region �̱���
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


        //uiManager = FindObjectOfType<InGameUIManager>();
        itemInfo = FindObjectOfType<ItemManager>();
        skillInfo = FindObjectOfType<SkillManager>();


        TryGetComponent(out itemInfo);
        TryGetComponent(out skillInfo);

        //playerData = DataManager.instance.PlayerDataGet(DataManager.instance.saveNumber);

        
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

        //uiManager.HpCheck(playerData.status.maxHp, playerData.status.currentHp);

        //QuickSlotItem = new GameObject[8];
    }
    private void Update()
    {
        //switch(currentView)
        //{
        //    case cameraView.ThirdPerson:
        //        {
        //            break;
        //        }
        //    case cameraView.FirstPerson:
        //        {
        //            break;
        //        }
        //}

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


    private void VeiwChange()
    {
        if (input.viewChange)
        {
            //[0]:3��Ī, [1]:1��Ī
            Debug.Log("�� ��ȯ");
            if (virtualCamera[0].activeSelf == true) //1��Ī���� ��ȯ
            {
                currentView = cameraView.FirstPerson;
                virtualCamera[0].SetActive(false);
                virtualCamera[1].SetActive(true);
            }
            else if (virtualCamera[1].activeSelf == true) //3��Ī���� ��ȯ
            {
                currentView = cameraView.ThirdPerson;
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
            case cameraView.ThirdPerson: //3��Ī ī�޶� ����
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
            case cameraView.FirstPerson: //1��Ī ī�޶� ����
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
        #region �÷��̾� ���ǵ� ����
        float targetSpeed = input.sprint ? sprintSpeed : moveSpeed;
        if (input.move == Vector2.zero)
        {
            targetSpeed = 0.0f;
        }

        float curHorizontalSpeed = new Vector3(charController.velocity.x, 0.0f, charController.velocity.z).magnitude;
        float speedOffset = 0.1f;

        //����,����
        if (curHorizontalSpeed > targetSpeed + speedOffset || curHorizontalSpeed < targetSpeed - speedOffset)
        {
            speed = Mathf.Lerp(curHorizontalSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        //���� �Ķ���� -> �����Ӽӵ�
        blend_MoveSpeed = Mathf.Lerp(blend_MoveSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
        if (blend_MoveSpeed < 0.1f)
        {
            blend_MoveSpeed = 0.0f;
        }
        #endregion

        Vector3 inputDirection = new Vector3(input.move.x, 0f, input.move.y).normalized;

        #region 3��Ī && 1��Ī �÷��̾� ���� ���� �� �����̱�
        switch (currentView)
        {
            case cameraView.ThirdPerson: //3��Ī
                {               
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
                    //�÷��̾� �����̱�
                    Vector3 targetDirection = Quaternion.Euler(0f, targetRotation, 0f) * Vector3.forward;
                    charController.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                                         new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
                    break;
                }
            case cameraView.FirstPerson: //1��Ī
                {
                    if (input.move != Vector2.zero)
                    {
                        inputDirection = transform.right * input.move.x + transform.forward * input.move.y;
                    }
                    //�÷��̾� �����̱�
                    charController.Move(inputDirection.normalized * (speed * Time.deltaTime) +
                        new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
                    break;
                }                
        }          
        #endregion
        
        //������ �ִϸ��̼�
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
    private void Attack() //���콺��Ŭ�� 
    {            
        switch (currentItem)
        {
            case ItemType.Empty:
                {
                    attackCool += Time.deltaTime;
                    canAttack = attackCool > 1f * 0.5f; //->1f�� staters.attackSpeed �� �ٲ����

                    if (input.attack && canAttack)
                    {
                        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
                        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

                        if (Physics.Raycast(ray, out RaycastHit hitInfo, 13f, LayerMask_Destroyable)) //������ ���̾��ũ
                        {
                            Debug.Log(hitInfo.transform.name);

                            if (Vector3.Distance(mainCamera.transform.position, rayPoint.position) <
                                Vector3.Distance(mainCamera.transform.position, hitInfo.transform.position))
                            {
                                if (hitInfo.transform.TryGetComponent(out BlockObject block))
                                {
                                    block.TakeDamage(30); //30�� staters.attack ���� �ٲ����
                                }
                            }
                            else //�÷��̾ ������Ʈ�� ���� �������� �Ⱥ��̴� ���
                            {
                                Debug.Log("��ĳ��");
                            }
                        }
                        else
                        {
                            Debug.Log("�����");
                        }

                        transform.rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
                        animator.SetTrigger(animID_Attack);
                        attackCool = 0;
                    }
                    break;
                }
            case ItemType.Sword:
                {
                    attackCool += Time.deltaTime;
                    canAttack = attackCool > 1f * equipItem.GetComponent<Sword>().attackRate; //->1f�� staters.attackSpeed �� �ٲ����

                    if (input.attack && canAttack)
                    {
                        transform.rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
                        animator.SetTrigger(animID_Swing);
                        equipItem.GetComponent<Sword>().Use();
                        attackCool = 0;
                    }
                    break;
                }
            case ItemType.Bow:
                {
                    break;
                }
            case ItemType.Potion:
                {
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

        playerData.status.currentHp -= damage - Mathf.RoundToInt(damage * Mathf.RoundToInt(100 * playerData.status.defens / (playerData.status.defens + 50)) * 0.01f);
        if(playerData.status.currentHp <= 0)

        {
            whenPlayerDie.Invoke();
        }
        //uiManager.HpCheck(status.maxHp, status.currentHp);
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
public class PlayerData  // �÷��̾� ������ ���� Ŭ����
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
public class Status  // �÷��̾� ���� ���� Ŭ����
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
public class Inventory  // �κ��丮 ���� ���� Ŭ����
{
    [XmlElement]
    public int tag;
    [XmlElement]
    public int quantity;
    [XmlElement]
    public bool hasItem;
}

[Serializable]
public class Skill  // ��ų ���� ���� Ŭ����
{
    [XmlElement]
    public int skillNum;
    [XmlElement]
    public int skillLevel;
    [XmlElement]
    public bool hasSkill;
}


