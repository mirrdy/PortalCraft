using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool hasItem = false;
    public new int tag = 0;
    public int quantity = 0;
    public string type = null;
    public int slotNumber;

    GameObject drag;
    bool isRightMouseDown = false;
    public int count = 1;

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

            if (eventData.clickCount == 2)
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
        if (hasItem)
        {
            uiManager.SlotNumberReset(tag, quantity, eventData.position);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (hasItem)
        {
            if (isRightMouseDown)
            {
                drag = eventData.pointerCurrentRaycast.gameObject;
                uiManager.dragAllocation(drag, tag, quantity, slotNumber);
            }
            uiManager.DragInItem(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (hasItem)
        {
            drag = eventData.pointerCurrentRaycast.gameObject;
            uiManager.DragDropItem(slotNumber, drag, type, tag, quantity);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            isRightMouseDown = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            isRightMouseDown = false;
        }
    }

    public void DragExit()
    {
        count = 1;
    }
}
