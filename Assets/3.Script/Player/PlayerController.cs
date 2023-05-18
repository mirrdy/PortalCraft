using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Movement3D movement3D;

    private void Awake()
    {
        TryGetComponent(out movement3D);
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        movement3D.MoveTo(new Vector3(x, 0, z));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement3D.Jump();
        }
    }
}
