using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input_Camera : MonoBehaviour
{
    public bool cursorInputForLook = true;
    public Vector2 look;
    public bool viewChange = false;


    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            look = value.Get<Vector2>();
        }
    }

    public void OnViewChange(InputValue value)
    {
        viewChange = value.isPressed;
    }
}
