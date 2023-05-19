using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharState : MonoBehaviour
{
    public enum PlayerState
    {
        Idle, Move, Attack
    }

    PlayerState _state = PlayerState.Idle;

    void UpdateIdle()
    {

    }

    void UpdateMove()
    {

    }

    void UpdateAttack()
    {

    }

    private void Update()
    {
        switch (_state)
        {
            case PlayerState.Idle:
                UpdateIdle();
                break;
            case PlayerState.Move:
                UpdateMove();
                break;
            case PlayerState.Attack:
                UpdateAttack();
                break;

        }
    }
}
