using Unity.Netcode;
using UnityEngine;

public class DripResourceManager : NetworkBehaviour
{
    public int wantedItemId;
    public GameObject createdItem;
    private bool startedFill;
    private float fillTimer;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;
        if (other.gameObject.GetComponentInParent<Item>() != null)
        {
            if (other.gameObject.GetComponentInParent<Item>().itemID != wantedItemId) return;
            fillTimer = 0;
            startedFill = true;
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (!IsServer) return;
        if (startedFill)
        {
            if (fillTimer < 5f) fillTimer += Time.deltaTime;
            else
            {
                SpawnItemServerRPC(other.transform.position);
                other.GetComponentInParent<NetworkObject>().Despawn(destroy: true);
                startedFill = false;
            }
        }
    }

    [ServerRpc]
    void SpawnItemServerRPC(Vector2 spawnPosition)
    {
        GameObject itemClone = Instantiate(createdItem, spawnPosition, Quaternion.identity);
        itemClone.GetComponent<Item>().wantedPositionOnSpawn = spawnPosition;
        itemClone.GetComponent<NetworkObject>().Spawn();
    }
}
