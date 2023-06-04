using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Xml.Serialization;

public class PlayerControl : MonoBehaviour, IDamage
{
    //ī�޶� ���� ������
    public enum CameraView { ThirdPerson = 0, FirstPerson = 1 };
    public CameraView currentView = CameraView.ThirdPerson;

    //���������� ������
    public enum ItemType { Empty = 0, Sword, Bow, Potion, Block, Torch }
    [SerializeField] private ItemType currentItem = ItemType.Empty;

    // --------------- ������Ʈ�� ----------------
    private Animator animator;
    private CharacterController charController;
    [HideInInspector]
    public Input_Info input;
    // ------------------------------------------

    // ---------------------------- ī�޶� ------------------------------- 
    [SerializeField] private GameObject mainCamera;
    public float cameraAngleOverride = 0.0f;
    // -------------------------------------------------------------------

    // �÷��̾�
    public float moveSpeed = 4.0f;
    public float sprintSpeed = 7.3f;
    public float jumpHeight = 2.5f;
    public float gravity = -30.0f;
    public float speedChangeRate = 10.0f; //����,����
    public float rotationSmoothTime = 0.12f;

    public bool grounded; //�÷��̾ ���� ��� �ִ���
    public float groundedRadius = 0.4f;
    public float groundedOffset = -0.3f;
    public float jumpCool = 0.3f; //�����ϰ� �ٽ� ���������ϱ� ������ �ð�
    public float fallTime = 0.15f; //�������� ���·� ��������� �ð� 

    public LayerMask LayerMask_Ground;
    public LayerMask LayerMask_Destroyable;
    public LayerMask layerMask_Block;

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

    #region �ִϸ��̼� �Ķ����ID
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
    private int animID_AttackSpeed;  
    #endregion

    #region ����Ⱥ���
    public int equip_HP;
    public int equip_Defense;
    public float equip_Speed;
    public int equip_Attack;
    public float equip_AttackRate;
    #endregion


    //public GameObject[] QuickSlotItem;
    [Header("���� �������� ������")]
    public GameObject equipItem;
    private Transform rayPoint;
    

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
        hasAnimator = transform.GetChild(0).TryGetComponent(out animator);
        animator = transform.GetChild(0).GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
        input = GetComponent<Input_Info>();

        jumpCoolDelta = jumpCool;
        fallTimeDelta = fallTime;

        uiManager.HpCheck(playerData.status.maxHp, playerData.status.currentHp);
        uiManager.ExpCheck((playerData.playerLevel * playerData.playerLevel - playerData.playerLevel) * 5 + 10, playerData.playerExp);

        #region �÷��̾� ��� �׽�Ʈ
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
        GroundCheck();
        Move();
        JumpAndGravity();
        DodgeRoll();
        Attack();
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

        inputDirection = new Vector3(input.move.x, 0f, input.move.y).normalized;

        #region 3��Ī && 1��Ī �÷��̾� ���� ���� �� �����̱�
        switch (currentView)
        {
            case CameraView.ThirdPerson: //3��Ī
                {               
                    if (input.move != Vector2.zero) //������O && ����X
                    {
                        targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
                        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);
                        transform.rotation = Quaternion.Euler(0f, rotation, 0f);
                    }
                    if (input.move != Vector2.zero && input.attack) //������O && ����O
                    {
                        if (currentItem != ItemType.Potion)
                        {
                            transform.rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
                        }
                    }
                    if (input.move == Vector2.zero && input.attack) //������X && ����O
                    {
                        if(currentItem != ItemType.Potion)
                        {
                            transform.rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
                        }                       
                    }
                    //�÷��̾� �����̱�
                    targetDirection = Quaternion.Euler(0f, targetRotation, 0f) * Vector3.forward;
                    charController.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                                         new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
                    break;
                }
            case CameraView.FirstPerson: //1��Ī
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
    private void DodgeRoll()
    {
        DodgeCoolDelta -= Time.deltaTime;
        canDodge = (DodgeCoolDelta <= 0);

        if (canDodge) // ������ ���� ����
        {
            if (Input.GetMouseButtonDown(1) && inputDirection.magnitude != 0 && grounded)
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
        ActionCool += Time.deltaTime;

        switch (currentItem) //�������� ���������
        {
            case ItemType.Empty:
                {
                    CanAction = ActionCool > 1f * 0.5f; //->1f�� staters.attackSpeed �� �ٲ����

                    if (input.attack && CanAction)
                    {
                        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
                        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

                        if (Physics.Raycast(ray, out RaycastHit hitInfo, 13f, LayerMask_Destroyable)) //������ ���̾��ũ
                        {
                            Debug.Log(hitInfo.transform.name);

                            if (Vector3.Distance(mainCamera.transform.position, rayPoint.position) <
                                Vector3.Distance(mainCamera.transform.position, hitInfo.transform.position))
                            {
                                if (hitInfo.transform.TryGetComponent(out IDestroyable obj))
                                {
                                    obj.TakeDamage(30); // 30 -> playerData.status.attack �Ŀ� ���� 
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

                        animator.SetTrigger(animID_Attack);
                        ActionCool = 0;
                    }
                    break;
                }
            case ItemType.Sword:
                {
                    float equipItem_attackRate = equipItem.GetComponent<Sword>().attackRate;
                    CanAction = ActionCool > 1f * equipItem_attackRate; // 1f -> playerData.status.attackRate �Ŀ� ����
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
                    CanAction = ActionCool > 1f * equipItem_attackRate; // 1f -> playerData.status.attackRate �Ŀ� ����
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
                    CanAction = ActionCool > 1f * 2.33f; //1f�� playerData.status.attackSpeed �� �־����
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
                    CanAction = ActionCool > 1f * 0.5f;
                    if (input.attack && CanAction)
                    {
                        CreateBlock();
                        ActionCool = 0;
                    }
                    break;
                }
            case ItemType.Torch:
                {
                    CanAction = ActionCool > 1f * 0.5f;
                    if (input.attack && CanAction)
                    {
                        ActionCool = 0;
                    }
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






    public void CreateBlock()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 20f, layerMask_Block))
        {
            //Debug.Log("�ش������Ʈ�� ��ǥ��" + hitInfo.transform.position);
            //Debug.Log("������ǥ��" + hitInfo.point);
            Vector3 vecDir = hitInfo.point - hitInfo.transform.position;
            //Debug.Log("�� ���Ͱ��� ���̴�" + vecDir);
            float xValue = vecDir.x;
            float yValue = vecDir.y;
            float zValue = vecDir.z;

            float maxValue = Mathf.Max(Mathf.Abs(xValue), Mathf.Abs(yValue), Mathf.Abs(zValue));
            if (Mathf.Abs(xValue) == maxValue)
            {
                //Debug.Log("vecDir.x�� ���� ū��");
                if (xValue > 0)
                {
                    Instantiate(equipItem, hitInfo.transform.position + Vector3.right, Quaternion.identity);
                }
                else if (xValue < 0)
                {
                    Instantiate(equipItem, hitInfo.transform.position + Vector3.left, Quaternion.identity);
                }
            }
            else if (Mathf.Abs(yValue) == maxValue)
            {
                //Debug.Log("vecDir.y�� ���� ū��");
                if (yValue > 0)
                {
                    Instantiate(equipItem, hitInfo.transform.position + Vector3.up, Quaternion.identity);
                }
                else if (yValue < 0)
                {
                    Instantiate(equipItem, hitInfo.transform.position + Vector3.down, Quaternion.identity);
                }
            }
            else if (Mathf.Abs(zValue) == maxValue)
            {
                //Debug.Log("vecDir.z�� ���� ū��");
                if (zValue > 0)
                {
                    Instantiate(equipItem, hitInfo.transform.position + Vector3.forward, Quaternion.identity);
                }
                else if (zValue < 0)
                {
                    Instantiate(equipItem, hitInfo.transform.position + Vector3.back, Quaternion.identity);
                }
            }
        }
    }
    public void OnDamage(int damage, Vector3 hitPosition, Vector3 hitNomal)
    {
        Status status = playerData.status;

        if (!isDodging) //�׽�Ʈ�غ�����
        {
            status.currentHp -= damage - Mathf.RoundToInt(damage * Mathf.RoundToInt(100 * status.defens / (status.defens + 50)) * 0.01f);
        }       

        if(status.currentHp <= 0) //�׾�����
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
    public void LevelUp() //�Ŀ� ���Ȼ�� �߰� 
    {
        playerData.playerExp -= (playerData.playerLevel * playerData.playerLevel - playerData.playerLevel) * 5 + 10;
        playerData.playerLevel++;
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
        animID_Roll = Animator.StringToHash("Roll");
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
    public string type;
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


