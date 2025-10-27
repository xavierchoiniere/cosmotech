
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetItemManager : NetworkBehaviour
{
    public List<GameObject> possibleDroppedItems;

    public void DropWantedItem(Vector2 playerPosition, int id, float x)
    {
        if (!IsServer) return;
        GameObject itemToSpawn = possibleDroppedItems.Find((item) => item.GetComponent<Item>().itemID == id);
        GameObject itemClone = Instantiate(itemToSpawn, playerPosition, Quaternion.identity);
        if (x > 0) itemClone.GetComponent<Item>().wantedPositionOnSpawn = new Vector2(playerPosition.x + 3, playerPosition.y);
        else itemClone.GetComponent<Item>().wantedPositionOnSpawn = new Vector2(playerPosition.x - 3, playerPosition.y);
        itemClone.GetComponent<Item>().justDropped = true;
        itemClone.GetComponent<Item>().dropTimer = 1f;
        itemClone.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DropItemServerRPC(Vector2 playerPosition, int id, float x)
    {
        DropWantedItem(playerPosition, id, x);
    }

}
