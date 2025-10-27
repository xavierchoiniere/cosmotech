using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class UIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public int itemID;
    public TextMeshProUGUI itemAmount;
    public PlayerInventory playerInventory;
    private BackpackManager backpackManager;
    private bool isAmountInitiated;
    private bool isInInventory;
    private bool isTool;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas parentCanvas;

    private int slotIndex;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        parentCanvas = GetComponentInParent<Canvas>();
        isInInventory = true;
    }

    void Start()
    {
        isAmountInitiated = false;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player != null)
            {
                if (player.GetComponent<PlayerInventory>() != null)
                {
                    playerInventory = player.GetComponent<PlayerInventory>();
                    backpackManager = playerInventory.backpackManager;
                    break;
                }
            }
        }
    }

    void Update()
    {
        isInInventory = transform.parent.parent.CompareTag("Slot Holder");
        if (transform.childCount > 1)
        {
            itemAmount = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            isAmountInitiated = true;
        }
        if (isInInventory && playerInventory.inventory.ContainsKey(itemID))
        {
            if (itemAmount.text != playerInventory.inventory[itemID].ToString() && isAmountInitiated)
            {
                if (itemID > 599 || itemID < 500) itemAmount.text = playerInventory.inventory[itemID].ToString();
                else itemAmount.text = Mathf.RoundToInt(transform.GetComponent<UniqueToolDurability>().currentDurability).ToString() + "%";
            }
        }
        else
        {
            if (int.Parse(itemAmount.text.Replace("%", "")) != transform.parent.GetComponent<Slot>().quantity && isAmountInitiated)
            {
                itemAmount.text = transform.parent.GetComponent<Slot>().quantity.ToString();
            }
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //add a way to drop tools
        if (eventData.button == PointerEventData.InputButton.Right) playerInventory.DropItem(itemID, int.Parse(itemAmount.text.Replace("%", "")), Input.GetKey(KeyCode.LeftShift)); 
        if (eventData.button == PointerEventData.InputButton.Left && Input.GetKey(KeyCode.LeftShift))
        {
            isTool = itemID <= 599 && itemID >= 500;
            slotIndex = rectTransform.parent.GetSiblingIndex();
            InstantAction();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                eventData.pointerDrag = null;
                return;
            }
            canvasGroup.blocksRaycasts = false;
            isTool = itemID <= 599 && itemID >= 500;
            playerInventory.gameObject.GetComponent<PlayerTool>().isInteractingWithUI = true;
            slotIndex = rectTransform.parent.GetSiblingIndex();
            rectTransform.parent.SetSiblingIndex(21);
            rectTransform.parent.parent.SetSiblingIndex(parentCanvas.GetComponent<RectTransform>().childCount);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (parentCanvas != null && eventData.button == PointerEventData.InputButton.Left)
        {
            rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        canvasGroup.blocksRaycasts = true;
        playerInventory.gameObject.GetComponent<PlayerTool>().isInteractingWithUI = false;

        PointerEventData pointerData = new(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointerData, results);

        GameObject slotTargetObj = results.Find(r => r.gameObject.CompareTag("Slot") || r.gameObject.CompareTag("Tool Slot")).gameObject;
        bool isToolSlot = slotTargetObj != null && slotTargetObj.CompareTag("Tool Slot");
        bool isInventorySlot = slotTargetObj != null && slotTargetObj.transform.parent.CompareTag("Slot Holder");

        if (slotTargetObj == null || (isToolSlot && !isTool)) //if you try to put in a tool slot and the item is not a tool or theres no slot
        {
            rectTransform.parent.SetSiblingIndex(slotIndex);
            rectTransform.localPosition = Vector3.zero;
            return;
        }

        Slot slotOrigin = rectTransform.parent.GetComponent<Slot>();
        bool isFromToolSlot = slotOrigin.transform.CompareTag("Tool Slot");
        Slot slotTarget = slotTargetObj.GetComponent<Slot>();
        UIItem targetItem = slotTargetObj.GetComponentInChildren<UIItem>();
        bool isTargetItemATool = targetItem != null && targetItem.itemID <= 599 && targetItem.itemID >= 500;

        rectTransform.parent.SetSiblingIndex(slotIndex);
       
        if (targetItem != null) //if slot has item
        {
            if (targetItem.itemID == itemID) //stack if same item on slot, return to set quantity correctly instead of "no matter what comment"
            {
                if (!isTool)
                {
                    if (isInventorySlot && !isInInventory) playerInventory.inventory[itemID] += slotOrigin.quantity;
                    if (!isInventorySlot)
                    {
                        slotTarget.quantity += int.Parse(itemAmount.text);
                        slotTarget.isChangedByUI = true;
                    }

                    if (isInInventory) backpackManager.RemoveSlot(slotIndex);
                    Destroy(targetItem.gameObject);

                    rectTransform.SetParent(slotTarget.transform);
                    rectTransform.localPosition = Vector3.zero;
                    slotOrigin.currentItemId = 0;
                    slotOrigin.quantity = 0;
                    slotOrigin.isChangedByUI = true;

                    return;
                }
                else if (isInventorySlot && isInInventory) //if its a tool
                {
                    if (isToolSlot) backpackManager.SwapItemSlots(slotIndex, 1);
                    else backpackManager.SwapItemSlots(slotIndex, targetItem.transform.parent.GetSiblingIndex());
                    slotOrigin.isChangedByUI = true;
                }
                
            }
            else //switch if not the same item on slot
            {
                if (!isTool)
                {
                    if (isInventorySlot && isInInventory) backpackManager.SwapItemSlots(slotIndex, targetItem.transform.parent.GetSiblingIndex());
                    slotOrigin.currentItemId = targetItem.itemID;
                    slotOrigin.quantity = int.Parse(targetItem.itemAmount.text.Replace("%", ""));
                    slotOrigin.isChangedByUI = true;
                }
                else if (isInventorySlot && isInInventory) //if its a tool
                {
                    if (isFromToolSlot && !isTargetItemATool)
                    {
                        rectTransform.parent.SetSiblingIndex(slotIndex);
                        rectTransform.localPosition = Vector3.zero;
                        return;
                    }
                    if (isToolSlot) backpackManager.SwapItemSlots(slotIndex, 1);
                    else backpackManager.SwapItemSlots(slotIndex, targetItem.transform.parent.GetSiblingIndex());
                    slotTarget.currentItemId = itemID;
                    slotOrigin.currentItemId = targetItem.itemID;
                    slotOrigin.isChangedByUI = true;
                }
               
            }
        }
        else //if slot is empty
        {
            if (isInventorySlot)
            {
                if (isInInventory) backpackManager.SwapItemSlots(slotIndex, slotTarget.transform.GetSiblingIndex()); //put item from inventory to empty slot in inventory
                else //add items if they didnt come from the inventory
                {
                    if (playerInventory.inventory.ContainsKey(itemID)) playerInventory.inventory[itemID] += slotOrigin.quantity;
                    else playerInventory.inventory[itemID] = slotOrigin.quantity;
                }
                slotOrigin.currentItemId = 0;
                slotOrigin.quantity = 0;
                slotOrigin.isChangedByUI = true;
            }
            else if (isInInventory) //move from inventory to empty slot outside of inventory, return to not set at 0 from "no matter what comment"
            {
                rectTransform.SetParent(slotTarget.transform);
                rectTransform.localPosition = Vector3.zero;
                slotTarget.currentItemId = itemID;
                slotTarget.quantity = int.Parse(itemAmount.text);
                slotTarget.isChangedByUI = true;

                playerInventory.inventory[itemID] -= int.Parse(itemAmount.text);
                if (playerInventory.inventory[itemID] != 0) backpackManager.RemoveSlot(slotIndex);

                return;
            }
        }

        //no matter what, set the current item's parent to the target and change the slot values
        rectTransform.SetParent(slotTarget.transform);
        rectTransform.localPosition = Vector3.zero;
        slotTarget.currentItemId = itemID;
        if (!isTool) slotTarget.quantity = int.Parse(itemAmount.text);
        slotTarget.isChangedByUI = true;
    }

    private void InstantAction()
    {
        if (playerInventory.GetComponent<PlayerNetwork>().indivStructManager.currentScreenStruct != null && 
            !playerInventory.GetComponent<PlayerNetwork>().indivStructManager.currentScreenStruct.GetComponent<ScreenIntStruct>().isScreenOn) return;
        GameObject currentScreen = null;
        if (playerInventory.GetComponent<PlayerNetwork>().indivStructManager.currentScreenStruct != null) 
            currentScreen = playerInventory.GetComponent<PlayerNetwork>().indivStructManager.currentScreenStruct.GetComponent<ScreenIntStruct>().screenTemp;
        if (currentScreen != null)
        {
            if (isInInventory)
            {
                List<Slot> slots = currentScreen.transform.Cast<Transform>().Where(child => child.CompareTag("Slot")).Select(child => child.GetComponent<Slot>()).ToList();
                foreach (Slot slot in slots)
                {
                    if (slot.possibleItemIds.Contains(itemID) && slot.GetComponentInChildren<UIItem>() == null)
                    {
                        rectTransform.parent.GetComponent<Slot>().currentItemId = 0;
                        rectTransform.parent.GetComponent<Slot>().quantity = 0;
                        rectTransform.parent.GetComponent<Slot>().isChangedByUI = true;
                        rectTransform.SetParent(slot.GetComponent<RectTransform>());
                        rectTransform.localPosition = Vector3.zero;
                        slot.currentItemId = itemID;
                        slot.quantity = int.Parse(itemAmount.text);
                        slot.isChangedByUI = true;

                        playerInventory.inventory[itemID] -= int.Parse(itemAmount.text);
                        if (playerInventory.inventory[itemID] != 0) backpackManager.RemoveSlot(slotIndex);

                        break;
                    }
                }
            }
            else
            {
                List<Slot> slots = backpackManager.slotHolder.GetComponentsInChildren<Slot>().ToList();
                bool noMatchingID = true;
                foreach (Slot slot in slots)
                {
                    if (slot.currentItemId == itemID)
                    {
                        rectTransform.parent.GetComponent<Slot>().currentItemId = 0;
                        rectTransform.parent.GetComponent<Slot>().quantity = 0;
                        rectTransform.parent.GetComponent<Slot>().isChangedByUI = true;
                        playerInventory.inventory[itemID] += int.Parse(itemAmount.text);
                        noMatchingID = false;
                        Destroy(gameObject);
                        break;
                    }
                }
                if (noMatchingID)
                {
                    rectTransform.parent.GetComponent<Slot>().currentItemId = 0;
                    rectTransform.parent.GetComponent<Slot>().quantity = 0;
                    rectTransform.parent.GetComponent<Slot>().isChangedByUI = true;
                    playerInventory.inventory[itemID] = int.Parse(itemAmount.text);
                    backpackManager.AddNewItem(itemID);
                    Destroy(gameObject);
                }
            }
        }
    }
}


