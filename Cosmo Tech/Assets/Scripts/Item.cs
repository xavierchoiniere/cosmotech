using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class Item : NetworkBehaviour
{
    public int itemID;
    public Vector2 wantedPositionOnSpawn;
    private bool isGoingTowardsPlayer;
    private GameObject player;
    private bool hasItemBeenPickedUp;

    public NetworkVariable<Vector2> wantedPositionOnSpawnNet = new NetworkVariable<Vector2>();
    public NetworkVariable<bool> itemSpawnedNet = new NetworkVariable<bool>(false);

    public bool justDropped;
    public float dropTimer;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            wantedPositionOnSpawnNet.Value = wantedPositionOnSpawn;
            itemSpawnedNet.Value = true;
        }
    }

    void Start()
    {
        if (wantedPositionOnSpawn == new Vector2(0,0)) wantedPositionOnSpawn = transform.position;
    }

    void Update()
    {
        if (itemSpawnedNet.Value)
        {
            wantedPositionOnSpawn = wantedPositionOnSpawnNet.Value;
        }
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        player = FindNearestPlayer(players);
        if (player != null) 
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer < 2f && !justDropped) isGoingTowardsPlayer = true;
            if (distanceToPlayer < 0.1f && isGoingTowardsPlayer && !hasItemBeenPickedUp)
            {
                if (player.GetComponent<PlayerInventory>() != null) //only update owner inventory
                {
                    Dictionary<int, int> playerInventory = player.GetComponent<PlayerInventory>().inventory;
                    if (playerInventory.ContainsKey(itemID)) playerInventory[itemID]++;
                    else playerInventory[itemID] = 1;
                    player.GetComponent<PlayerInventory>().UpdateInventory();
                    hasItemBeenPickedUp = true;
                }
                if (IsServer) GetComponent<NetworkObject>().Despawn(destroy: true);
            }
            if (distanceToPlayer > 0.1f && isGoingTowardsPlayer)
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, Time.deltaTime * 10f);

            if (Vector2.Distance(transform.position, wantedPositionOnSpawn) > 0.1f && !isGoingTowardsPlayer)
            {
                float wantedSpeed = Time.deltaTime * 5f;
                if (justDropped) wantedSpeed = Time.deltaTime * 10f;
                transform.position = Vector2.MoveTowards(transform.position, wantedPositionOnSpawn, wantedSpeed);
            }
                
        }  
        if (justDropped)
        {
            if (dropTimer > 0) dropTimer -= Time.deltaTime;
            else justDropped = false;
        }
    }
    GameObject FindNearestPlayer(GameObject[] players)
    {
        GameObject nearestPlayer = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject potentialPlayer in players)
        {
            float distance = Vector2.Distance(transform.position, potentialPlayer.transform.position);
            if (distance < minDistance)
            {
                nearestPlayer = potentialPlayer;
                minDistance = distance;
            }
        }

        return nearestPlayer;
    }
}
