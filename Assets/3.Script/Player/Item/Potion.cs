using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public int healAmount;  //회복량

    private InGameUIManager uiManager;

    private void Start()
    {
        uiManager = GameObject.Find("InteractionManager").GetComponent<InGameUIManager>();
    }

    public void Use(int slotNumber, int tag)
    {
        Status playerData = PlayerControl.instance.playerData.status;

        if (tag == 501)
        {
            uiManager.CraftingBlock(slotNumber);
            playerData.currentHp += 30;
            if(playerData.currentHp >= playerData.maxHp)
            {
                playerData.currentHp = playerData.maxHp;
            }
        }
        else if (tag == 502)
        {
            uiManager.CraftingBlock(slotNumber);
            playerData.currentMp += 30;
            if (playerData.currentMp >= playerData.maxMp)
            {
                playerData.currentMp = playerData.maxMp;
            }
        }
    }

    IEnumerator DrinkPotion()
    {
        yield return new WaitForSeconds(1f);
        //PlayerControl.instance.playerData.status.currentHp += healAmount;
        Debug.Log("체력" + healAmount + "회복");
    }
}
