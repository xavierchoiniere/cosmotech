using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class RecipeEntry
{
    public int neededItemID;
    public int amount;
}


public class RecipeManager : MonoBehaviour
{
    public List<RecipeEntry> recipeEntries;
    public GameObject result;
    public int resultAmount;

    private StructManager structManager;
    private PlayerInventory playerInventory;

    void Start()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (!player.GetComponent<PlayerNetwork>().IsLocalPlayer) continue;
            playerInventory = player.GetComponent<PlayerInventory>();
            structManager = player.GetComponent<PlayerNetwork>().indivStructManager;

        }
    }

    public void Craft()
    {
        bool isValidForCrafting = true;
        for (int i = 0; i < recipeEntries.Count; i++)
        {
            if (!playerInventory.inventory.ContainsKey(recipeEntries[i].neededItemID))
            {
                isValidForCrafting = false;
                break;
            }
            if (playerInventory.inventory[recipeEntries[i].neededItemID] < recipeEntries[i].amount)
            {
                isValidForCrafting = false;
                break;
            }
        }
        if (isValidForCrafting)
        {
            Dictionary<int, int> playerInv = playerInventory.inventory;
            foreach (var recipeEntry in recipeEntries)
            {
                playerInv[recipeEntry.neededItemID] -= recipeEntry.amount;
                playerInventory.UpdateInventory();
            }
            if (result.GetComponent<Item>()) CreateItem(playerInv);
            else if (result.GetComponent<InteractableStructure>()) CreateStructure();
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void CreateItem(Dictionary<int, int> inventory)
    {
        if (inventory.ContainsKey(result.GetComponent<Item>().itemID)) inventory[result.GetComponent<Item>().itemID] += resultAmount;
        else inventory[result.GetComponent<Item>().itemID] = resultAmount;
        playerInventory.UpdateInventory();
    }

    private void CreateStructure()
    {
        structManager.isPlacingStruct = true;
        GameObject structure = Instantiate(result, GameObject.FindGameObjectWithTag("Global Struct Manager").transform);
        structManager.currentlyPlacingStruct = structure.GetComponent<InteractableStructure>();
    }
}

