using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement3D : MonoBehaviour
{
    private Vector3 moveDirection;   // �̵� ����
    public float moveSpeed = 5.0f;   // �̵� �ӵ�
    public float gravity = -6f;      // �߷� ���
    public float jumpForce = 2f;     // ������

    private float targetAngle;

    public float smoothTime = 0.05f;
    private float smoothVelocity;

    private CharacterController charController;
    private Animator anim;
    public Transform cam;

    float idle_move_ratio;

    private void Awake()
    {
        TryGetComponent(out charController);
        anim = transform.GetChild(1).GetComponent<Animator>();
    }

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {        
        //ĳ���� ������ && �ٶ󺸴¹���
        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            idle_move_ratio = Mathf.Lerp(idle_move_ratio, 1, 15.0f * Time.deltaTime);
            anim.SetFloat("idle_move_ratio", idle_move_ratio);
            anim.Play("Idle_Move");

            targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            charController.Move(moveDirection * moveSpeed * Time.deltaTime); 
        }
        else
        {
            idle_move_ratio = Mathf.Lerp(idle_move_ratio, 0, 15.0f * Time.deltaTime);
            anim.SetFloat("idle_move_ratio", idle_move_ratio);
            anim.Play("Idle_Move");
        }
                             
        //�߷±���
        if (!charController.isGrounded)
        {
            charController.Move(new Vector3(0, gravity, 0) * Time.deltaTime);
        }    
    }

    public void MoveTo(Vector3 direction)
    {
        moveDirection = new Vector3(direction.x, moveDirection.y, direction.z);
    }
    
    public void Jump()
    {
        if (charController.isGrounded)
        {
            charController.Move(new Vector3(0, jumpForce, 0));
        }
    }
}
