using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement3D : MonoBehaviour
{
    private Vector3 moveDirection;   // 이동 방향
    public float moveSpeed = 5.0f;   // 이동 속도
    public float gravity = -6f;      // 중력 계수
    public float jumpForce = 2f;   // 점프력

    private CharacterController charController;

    private void Awake()
    {
        TryGetComponent(out charController);
    }

    private void Update()
    {
        float moveAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, moveAngle, 0);

        if (!charController.isGrounded)
        {
            moveDirection.y += gravity * Time.deltaTime;
        }
   
        charController.Move(moveDirection * moveSpeed * Time.deltaTime); //움직이는 부분
    }

    public void MoveTo(Vector3 direction)
    {
        moveDirection = new Vector3(direction.x, moveDirection.y, direction.z);
    }
    
    public void Jump()
    {
        if (charController.isGrounded)
        {
            moveDirection.y = jumpForce;
        }
    }
}
