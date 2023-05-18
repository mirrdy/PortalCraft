using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement3D : MonoBehaviour
{
    private Vector3 moveDirection;   // �̵� ����
    public float moveSpeed = 5.0f;   // �̵� �ӵ�
    public float gravity = -6f;      // �߷� ���
    public float jumpForce = 2f;   // ������

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
   
        charController.Move(moveDirection * moveSpeed * Time.deltaTime); //�����̴� �κ�
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
