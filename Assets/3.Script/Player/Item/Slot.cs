using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public bool hasItem = false;
    public int tag = 0;
    public int quantity = 0;
    public string type = null;
    public int slotNumber;

    float currentTimeClick;
    float lastTimeClick;

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
                if(slotNumber == 38)
                {
                    uiManager.AddItem(tag, type, quantity, slotNumber);
                    return;
                }
                else if (slotNumber == 39)
                {
                    uiManager.AddItem(tag, type, quantity, slotNumber);
                    return;
                }
                else if (slotNumber == 40)
                {
                    uiManager.AddItem(tag, type, quantity, slotNumber);
                    return;
                }

                if (type.Equals("Helmet"))
                {
                    uiManager.ChangedItme(slotNumber, 38);
                }
                else if (type.Equals("Armor"))
                {
                    uiManager.ChangedItme(slotNumber, 39);
                }
                else if (type.Equals("Cloak"))
                {
                    uiManager.ChangedItme(slotNumber, 40);
                }
                else if (type.Equals("About"))
                {

                }
            }
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            currentTimeClick = Time.time;
            if (currentTimeClick - lastTimeClick >= 0.8f)
            {
                lastTimeClick = Time.time;
            }
            if (Mathf.Abs(currentTimeClick - lastTimeClick) < 0.75f)
            {
                if (hasItem)
                {
                    if (slotNumber == 38)
                    {
                        uiManager.AddItem(tag, type, quantity, slotNumber);
                        return;
                    }
                    else if (slotNumber == 39)
                    {
                        uiManager.AddItem(tag, type, quantity, slotNumber);
                        return;
                    }
                    else if (slotNumber == 40)
                    {
                        uiManager.AddItem(tag, type, quantity, slotNumber);
                        return;
                    }

                    if (type.Equals("Helmet"))
                    {
                        uiManager.ChangedItme(slotNumber, 38);
                    }
                    else if (type.Equals("Armor"))
                    {
                        uiManager.ChangedItme(slotNumber, 39);
                    }
                    else if (type.Equals("Cloak"))
                    {
                        uiManager.ChangedItme(slotNumber, 40);
                    }
                    else if (type.Equals("About"))
                    {

                    }
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
