using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input_Info : MonoBehaviour
{
    [Header("ĳ���� Input ��")]
    public Vector2 move;
    public Vector2 look;
    public bool sprint;
    public bool jump;
    public bool attack;
    public bool skill_1;
    public bool skill_2;
    public bool viewChange;

    public Vector2 cameraLook;

    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            look = value.Get<Vector2>();
        }            
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

    public void OnSkill_1(InputValue value)
    {
        skill_1 = value.isPressed;
    }
    public void OnSkill_2(InputValue value)
    {
        skill_2 = value.isPressed;
    }

    public void OnViewChange(InputValue value)
    {
        viewChange = value.isPressed;
    }

    public void OnCameraLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            cameraLook = value.Get<Vector2>();
        }
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
