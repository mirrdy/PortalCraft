using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    //public int healAmount;  //È¸º¹·®

    private InGameUIManager uiManager;


    public void Use(int slotNumber, int tag)
    {
        uiManager = GameObject.Find("InGameUIManager").GetComponent<InGameUIManager>();

        Status playerData = PlayerControl.instance.playerData.status;

        if (tag == 501)
        {
            uiManager.CraftingBlock(slotNumber);
            playerData.currentHp += 30;
            if(playerData.currentHp >= playerData.maxHp)
            {
                playerData.currentHp = playerData.maxHp;
            }
            uiManager.HpCheck(playerData.maxHp, playerData.currentHp);
        }
        else if (tag == 502)
        {
            uiManager.CraftingBlock(slotNumber);
            playerData.currentMp += 30;
            if (playerData.currentMp >= playerData.maxMp)
            {
                playerData.currentMp = playerData.maxMp;
            }
            uiManager.MpCheck(playerData.maxMp, playerData.currentMp, 0, 0);
        }
    }
}
