using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static ToolStats;

public class Resource : NetworkBehaviour
{
    public float maxResourceHP;
    public List<GameObject> wantedItemsOnBreak;
    public int[] wantedItemsAmounts;
    public mineTypeOption resourceType;
    public bool canDropWhileHit;
    public bool isBreakableByWalk;
    public GameObject wantedItemOnHit;

    public int rarity;
    public List<Vector2> tilesTaken;

    private float currentResourceHP;
    private Animator animator;

    void Start()
    {
        currentResourceHP = maxResourceHP;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (currentResourceHP <= 0)
        {
            if (IsServer)
            {
                for (int i = 0; i < wantedItemsOnBreak.Count; i++)
                {
                    for (int j = 0; j < wantedItemsAmounts[i]; j++)
                    {
                        float randomNumberX = Random.Range(-1f, 2f);
                        float randomNumberY = Random.Range(-0.5f, 0.5f);
                        SpawnItemServerRPC(new Vector2(transform.position.x + randomNumberX, transform.position.y + randomNumberY), i);
                    }
                }
            }
            List<Vector2> tilesOccupied = new List<Vector2>();
            foreach (var tilePos in tilesTaken) tilesOccupied.Add(new Vector2(tilePos.x + transform.position.x, tilePos.y + transform.position.y));
            GameObject.FindGameObjectWithTag("Tile Manager").GetComponent<TileManager>().ManageGivenTiles(tilesOccupied, false);
            Destroy(gameObject);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("resource_hit") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) animator.SetBool("IsHit", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ToolStats toolStats = other.transform.GetComponent<ToolStats>();
        if (animator.GetBool("IsHit")) return;
        if (toolStats)
        {
            if (toolStats.mineType == resourceType) currentResourceHP -= toolStats.resourceDamage;
            else currentResourceHP -= toolStats.resourceDamage * 0.3f;
            animator.SetBool("IsHit", true);
            GameObject toolSlot = GameObject.FindGameObjectWithTag("Tool Slot");
            if (toolSlot != null && toolSlot.transform.childCount > 0) //only update owner durability
            {
                if (toolSlot.transform.GetChild(0).GetComponent<UniqueToolDurability>() != null)
                {
                    UniqueToolDurability uniqueToolDurability = toolSlot.transform.GetChild(0).GetComponent<UniqueToolDurability>();
                    if (uniqueToolDurability != null)
                    {
                        uniqueToolDurability.currentDurability -= uniqueToolDurability.durabilityLoss;
                    }
                }
            }
            if (canDropWhileHit && IsServer)
            {
                int r = Random.Range(0, 5);
                if (r == 0)
                {
                    float randomNumberX = Random.Range(-3f, 3f);
                    float randomNumberY = Random.Range(-1f, 1f);
                    SpawnItemServerRPC(new Vector2(transform.position.x + randomNumberX, transform.position.y + randomNumberY), 9999);
                }
            }
        }
        else if (other.transform.GetComponent<PlayerMovement>() && isBreakableByWalk) 
        {
            currentResourceHP -= 1;
            animator.SetBool("IsHit", true);
        }

    }

    [ServerRpc]
    void SpawnItemServerRPC(Vector2 spawnPosition, int index)
    {
        GameObject itemToSpawn = wantedItemOnHit;
        if (index != 9999) itemToSpawn = wantedItemsOnBreak[index];
        GameObject itemClone = Instantiate(itemToSpawn, transform.position, Quaternion.identity);
        itemClone.GetComponent<Item>().wantedPositionOnSpawn = spawnPosition;
        itemClone.GetComponent<NetworkObject>().Spawn();
    }
}
