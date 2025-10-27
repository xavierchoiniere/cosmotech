using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class BackpackManager : MonoBehaviour
{
    public List<GameObject> possibleUIItems;
    public GameObject itemAmount;
    public Transform slotHolder;
    public Transform toolSlot;
    void Start()
    {
        toolSlot = slotHolder.GetChild(1);
    }

    public void SwapItemSlots(int draggedSlotIndex, int targetSlotIndex)
    {
        Transform draggedItem = slotHolder.GetChild(draggedSlotIndex).GetComponentInChildren<UIItem>().transform;
        Transform targetSlot = slotHolder.GetChild(targetSlotIndex);
        if (targetSlot.childCount > 0)
        {
            draggedItem.SetParent(targetSlot.transform);
            Transform targetItem = targetSlot.GetComponentInChildren<UIItem>().transform;
            targetItem.SetParent(slotHolder.GetChild(draggedSlotIndex));
            draggedItem.localPosition = Vector3.zero;
            targetItem.localPosition = Vector3.zero;
        }
    }

    public bool IsItemShown(int itemID)
    {
        bool returnedValue = false;
        foreach (Transform slot in slotHolder)
        {
            if (slot.GetComponentInChildren<UIItem>() != null)
            {
                if (slot.GetComponentInChildren<UIItem>().itemID == itemID)
                {
                    returnedValue = true;
                    break;
                }
            }
        }
        return returnedValue;
    }

    public void AddNewItem(int itemID)
    {
        bool isToolSlotFree = toolSlot.childCount == 0;       
        bool isToolItem = itemID >= 500 && itemID <= 599;

        if (isToolItem && isToolSlotFree)
        {
            AddItemToSlot(itemID, 1);
            return;
        }

        for (int i = 2; i <= 21; i++)
        {
            if (slotHolder.GetChild(i).childCount == 0)
            {
                AddItemToSlot(itemID, i);
                break;
            }
        }
    }

    public void RemoveEquippedTool()
    {
        Destroy(toolSlot.GetChild(0).gameObject);
    }

    public void RemoveItem(int itemID) 
    {
        for (int i = 2; i <= 21; i++)
        {
            if (slotHolder.GetChild(i).GetComponentInChildren<UIItem>() != null)
            {
                if (slotHolder.GetChild(i).GetComponentInChildren<UIItem>().itemID == itemID)
                {
                    RemoveSlot(i);
                    break;
                }
            }
        }
    }

    public void RemoveSlot(int slotIndex)
    {
        Destroy(slotHolder.GetChild(slotIndex).GetComponentInChildren<UIItem>().gameObject);
    }

    private void AddItemToSlot(int itemID, int slotIndex)
    {
        GameObject wantedItem = possibleUIItems.Find(x => x.GetComponent<UIItem>().itemID == itemID);
        GameObject itemImage = Instantiate(wantedItem, slotHolder.GetChild(slotIndex), false);
        GameObject amount = Instantiate(itemAmount, itemImage.transform, false);
        slotHolder.GetChild(slotIndex).GetComponent<Slot>().currentItemId = itemID;

        if (itemID >= 500 && itemID <= 599)
            amount.transform.localPosition = new Vector2(0, -20);
    }
}
