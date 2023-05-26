using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input_Info : MonoBehaviour
{
    [Header("캐릭터 Input 값")]
    public Vector2 move;
    public Vector2 look;
    public bool sprint;
    public bool jump;

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        sprint = value.isPressed;
    }

    public void OnJump(InputValue value)
    {
        jump = value.isPressed;
    }
}
