using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Movement3D movement3D;
    private CharacterController charController;

    private void Awake()
    {
        TryGetComponent(out movement3D);
        TryGetComponent(out charController);
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        movement3D.MoveTo(new Vector3(x, 0, z));

        if (Input.GetKeyDown(KeyCode.Space) && charController.isGrounded)
        {
            movement3D.Jump();
        }
    }
}
