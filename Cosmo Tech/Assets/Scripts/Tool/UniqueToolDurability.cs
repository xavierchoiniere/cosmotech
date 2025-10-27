using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueToolDurability : MonoBehaviour
{
    public float currentDurability;
    public float durabilityLoss;
    public BackpackManager backpackManager;

    void Start()
    {
        backpackManager = GameObject.FindGameObjectWithTag("BPManager").GetComponent<BackpackManager>();
    }

    void Update()
    {
        if (currentDurability <= 0)
        {
            PlayerInventory playerInv = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
            playerInv.inventory[transform.GetComponent<UIItem>().itemID]--;
            playerInv.UpdateInventory(playerInv.inventory[transform.GetComponent<UIItem>().itemID]);
            backpackManager.RemoveEquippedTool();
        }
    }
}
