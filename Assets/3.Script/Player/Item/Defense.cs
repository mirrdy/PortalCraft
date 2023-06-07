using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour
{
    public int HP;
    public int defense;
    public float moveSpeed;


    private void OnEnable()
    {
        PlayerControl.instance.playerData.status.maxHp += HP;
        PlayerControl.instance.playerData.status.defens += defense;
        PlayerControl.instance.playerData.status.moveSpeed += moveSpeed;
    }
    private void OnDisable()
    {
        PlayerControl.instance.playerData.status.maxHp -= HP;
        PlayerControl.instance.playerData.status.defens -= defense;
        PlayerControl.instance.playerData.status.moveSpeed -= moveSpeed;
    }
}
