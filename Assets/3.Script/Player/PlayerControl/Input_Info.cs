using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input_Info : MonoBehaviour
{
    [Header("ĳ���� Input ��")]
    public Vector2 move;
    public bool sprint;
    public bool jump;
    public bool attack;
    public bool mouse_1;
    public bool skill_1;
    public bool skill_2;

    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        sprint = value.isPressed;
    }

    public void OnJump(InputValue value)
    {
        jump = value.isPressed;
    }

    public void OnAttack(InputValue value)
    {
        attack = value.isPressed;
    }
    public void OnBuild(InputValue value)
    {
        mouse_1 = value.isPressed;
    }

    public void OnSkill_1(InputValue value)
    {
        skill_1 = value.isPressed;
    }
    public void OnSkill_2(InputValue value)
    {
        skill_2 = value.isPressed;
    }




    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
