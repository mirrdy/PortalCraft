using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    public bool hasItem = false;
    public int tag = 0;
    public int quantity = 0;
    public string type = null;
    public int slotNumber;

    private InGameUIManager uiManager;

    private void Awake()
    {
        uiManager = FindObjectOfType<InGameUIManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if (hasItem)
            {
                if (type.Equals("Helmet"))
                {
                    uiManager.ChangedHelmet(slotNumber);
                }
                else if (type.Equals("Armor"))
                {
                    uiManager.ChangedAramor(slotNumber);
                }
                else if (type.Equals("About"))
                {

                }
            }
        }
    }
}
