using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : InteractiveEntity
{
    public override void Interact()
    {
        if(GameObject.Find("InGameUIManager").TryGetComponent(out InGameUIManager ui))
        {
            ui.OnCraftTable();
            if(GameObject.Find("InteractionManager").TryGetComponent(out InteractionManager interactionManager))
            {
                interactionManager.isInteracting = true;
            }
        }
    }
}
