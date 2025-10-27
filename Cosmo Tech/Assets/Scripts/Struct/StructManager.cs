using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class StructManager : MonoBehaviour
{
    public InteractableStructure nearestStruct;
    public InteractableStructure currentlyPlacingStruct;
    public InteractableStructure currentScreenStruct;
    public Transform cursor;
    public GameObject respectivePlayer;
    public NetStructManager globalStructManager;

    public bool isPlacingStruct;
    private float distance;
    public GameObject[] structs;
    private Vector2 lastPlayerPosition;

    public TileManager tileManager;

    void Start()
    {
        nearestStruct = null;
        currentlyPlacingStruct = null;
        globalStructManager = GameObject.FindGameObjectWithTag("Global Struct Manager").GetComponent<NetStructManager>();
        structs = GameObject.FindGameObjectsWithTag("Structure");
        distance = Mathf.Infinity;
    }

    void Update()
    {
        
        if (respectivePlayer != null)
        {
            Vector2 playerPos = respectivePlayer.transform.position;
            if (Vector2.Distance(playerPos, lastPlayerPosition) > 0.1f || structs.Length != GameObject.FindGameObjectsWithTag("Structure").Length)
            {
                structs = GameObject.FindGameObjectsWithTag("Structure");
                distance = Mathf.Infinity;
                foreach (GameObject s in structs)
                {
                    if (s.GetComponent<InteractableStructure>() == null) continue;
                    if (respectivePlayer == null) continue;
                    if (distance > Vector2.Distance(respectivePlayer.transform.position, s.transform.position))
                    {
                        nearestStruct = s.GetComponent<InteractableStructure>();
                        distance = Vector2.Distance(respectivePlayer.transform.position, s.transform.position);
                    }
                }
                lastPlayerPosition = playerPos;
            }
        }
       

        if (currentlyPlacingStruct != null)
        {
            if (currentScreenStruct is ScreenIntStruct && currentScreenStruct.GetComponent<ScreenIntStruct>().isScreenOn) currentScreenStruct.GetComponent<ScreenIntStruct>().SwitchInventoryState();
            if (!isPlacingStruct) isPlacingStruct = true;
            if (!tileManager.isShowingAllTiles) tileManager.SwitchShowTiles();
            if (nearestStruct.interaction != null)
            {
                if (nearestStruct.interaction.gameObject.activeInHierarchy) nearestStruct.interaction.gameObject.SetActive(false);
            }

            Color color = new Color(1f, 1f, 1f, 0.75f);
            currentlyPlacingStruct.GetComponentInChildren<SpriteRenderer>().color = color;
            foreach (Collider2D collider in currentlyPlacingStruct.GetComponentsInChildren<Collider2D>()) collider.enabled = false;
            respectivePlayer.GetComponent<PlayerTool>().isInteractingWithUI = true;

            Vector2 bottomLeft = currentlyPlacingStruct.GetComponentInChildren<SpriteRenderer>().bounds.min;
            currentlyPlacingStruct.transform.position = new Vector2(Mathf.Round(cursor.transform.position.x), Mathf.Round(cursor.transform.position.y));

            List<Vector2> occupiedTiles = MakeOccupiedTileList(new Vector2(Mathf.Round(bottomLeft.x), Mathf.Round(bottomLeft.y)), currentlyPlacingStruct.occupiedTiles);

            if (Input.GetMouseButtonDown(0) && tileManager.AreAllTilesFree(occupiedTiles))
            {
                globalStructManager.RequestStructureSpawnServerRpc(currentlyPlacingStruct.name.Replace("(Clone)", ""), currentlyPlacingStruct.transform.position);
                FinishPlacing(occupiedTiles);
            }
        }
    }

    private List<Vector2> MakeOccupiedTileList(Vector2 basePosition, List<Vector2> tilePositions)
    {
        List<Vector2> returnedList = new();
        foreach (Vector2 tilePosition in tilePositions) returnedList.Add(basePosition + tilePosition);
        return returnedList;
    }

    private void FinishPlacing(List<Vector2> tilesToOccupy)
    {
        tileManager.SwitchShowTiles();
        foreach (Vector2 pos in tilesToOccupy) 
        {
            TileData tileData = tileManager.tileDataDict[new Vector2(pos.x, pos.y)];
            tileData.isOccupied = true;
        }
        Destroy(currentlyPlacingStruct.gameObject);
        isPlacingStruct = false;
        respectivePlayer.GetComponent<PlayerTool>().isInteractingWithUI = false;
        currentlyPlacingStruct = null;
    }
}
