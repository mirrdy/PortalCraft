using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 4.0f;
    public float sprintSpeed = 7.3f;
    public float jumpHeight = 2.3f;
    public float gravity = -15.0f;
    public float speedChangeRate = 10.0f; //����,����
    public float rotationSmoothTime = 0.12f;

    public bool grounded; //�÷��̾ ���� ��� �ִ°�?
    public float groundedRadius = 0.4f;
    public float groundedOffset = -0.3f;
    public float jumpCool = 0.3f; //�����ϰ� �ٽ� ���������ϱ� ������ �ð�
    public float fallTime = 0.15f; //�������� ���·� ��������� �ð� 

    public LayerMask groundLayers;

    //ī�޶� ���� 
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

    //�ִϸ��̼� �Ķ���� ID
    private int animID_Speed;
    private int animID_Ground;
    private int animID_Jump;
    private int animID_Falling;
    private int animID_Attack;
    private int animID_Swing;
    private int animID_Shot;

    private bool canAttack;
    public float weaponAttackCool;
    public float 
    public Weapon equipWeapon;



    private void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
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
    }

    private void Update()
    {
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




    private void GroundCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);

        grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

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

        //������ �Ķ���� -> �����Ӽӵ�
        blend_MoveSpeed = Mathf.Lerp(blend_MoveSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
        if (blend_MoveSpeed < 0.1f)
        {
            blend_MoveSpeed = 0.0f;
        }
        #endregion

        #region �÷��̾� ���� ����
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

        //�÷��̾� �����̱� 
        Vector3 targetDirection = Quaternion.Euler(0f, targetRotation, 0f) * Vector3.forward;
        charController.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                             new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

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
            // reset the fall timeout timer
            fallTimeDelta = fallTime;

            // update animator if using character
            if (hasAnimator)
            {
                animator.SetBool(animID_Jump, false);
                animator.SetBool(animID_Falling, false);
            }

            // stop our velocity dropping infinitely when grounded
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }

            // Jump
            if (input.jump && jumpCoolDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                // update animator if using character
                if (hasAnimator)
                {
                    animator.SetBool(animID_Jump, true);
                }
            }

            // jump timeout
            if (jumpCoolDelta >= 0.0f)
            {
                jumpCoolDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            jumpCoolDelta = jumpCool;

            // fall timeout
            if (fallTimeDelta >= 0.0f)
            {
                fallTimeDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                if (hasAnimator)
                {
                    animator.SetBool(animID_Falling, true);
                }
            }

            // if we are not grounded, do not jump
            input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void Attack()
    {
        weaponAttackCool += Time.deltaTime;
        canAttack = equipWeapon.rate < weaponAttackCool;

        if (input.attack && equipWeapon == null)
        {
            animator.SetTrigger(animID_Attack);
        }

        if (equipWeapon.type == Weapon.Type.Melee && input.attack && canAttack)
        {
            equipWeapon.Use();
            animator.SetTrigger(animID_Swing);
            weaponAttackCool = 0;
        }

        if (input.attack && equipWeapon.type == Weapon.Type.Range && canAttack)
        {
            equipWeapon.Use();
            animator.SetTrigger(animID_Shot);
            weaponAttackCool = 0;
        }
        

        
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

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
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