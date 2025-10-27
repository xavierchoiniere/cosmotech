using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerInventory : MonoBehaviour
{
    public BackpackManager backpackManager;
    public Dictionary<int, int> inventory;
    private Dictionary<int, int> previousSnapshot = new Dictionary<int, int>();
    public NetItemManager netItemManager;

    void Start()
    {
        inventory = new Dictionary<int, int>();
        netItemManager = GameObject.FindGameObjectWithTag("Global Item Manager").GetComponent<NetItemManager>();
    }

    void Update()
    {
        foreach (var key in inventory.Keys)
        {
            if (inventory[key] == 0)
            {
                backpackManager.RemoveItem(key);
                inventory.Remove(key);
                break;
            }
        }
    }
    public void UpdateInventory(int? numberOfTools = 0)
    {
        foreach (var key in inventory.Keys)
        {
            bool isToolItem = key >= 500 && key <= 599;
            if (!previousSnapshot.ContainsKey(key) || previousSnapshot[key] != inventory[key])
            {
                if (!backpackManager.IsItemShown(key) && !isToolItem && inventory[key] > 0) backpackManager.AddNewItem(key);
                if (isToolItem && inventory[key] > numberOfTools) backpackManager.AddNewItem(key);
                if (inventory[key] == 0)
                {
                    if (!isToolItem) backpackManager.RemoveItem(key);
                    inventory.Remove(key);
                }
                previousSnapshot = new Dictionary<int, int>(inventory);
                break;
            }
        }
    }

    public void DropItem(int droppedItemID, int itemAmount, bool isShift) //add a way to drop tools
    {
        if (inventory.ContainsKey(droppedItemID))
        {
            if (!isShift) itemAmount = 1;
            for (int i = 0; i < itemAmount; i++)
            {
                netItemManager.DropItemServerRPC(transform.position, droppedItemID, transform.localScale.x);
                --inventory[droppedItemID];
                UpdateInventory();
            }
            
        }
    }

    public bool HasItemId(int itemId)
    {
        return inventory != null && inventory.ContainsKey(itemId);
    }
}
