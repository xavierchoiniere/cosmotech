using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceSpawningManager : MonoBehaviour
{
    public TileManager tileManager;
    public int resourceExpansionRatio;
    public List<GameObject> possibleResources;
    public WorldGenerator worldGenerator;

    private int currentResourceCount;
    private BoundsInt tilemapBounds;
    private bool spawningStarted = false;
    public bool spawningFinished = false;

    private void Start()
    {
        tilemapBounds = tileManager.map.cellBounds;
    }

    void Update()
    {
        if (!spawningStarted && tileManager.tilesInitiated)
        {
            spawningStarted = true;
            StartCoroutine(SpawnResourcesCoroutine());
        }
    }

    private IEnumerator SpawnResourcesCoroutine()
    {
        tilemapBounds = tileManager.map.cellBounds;
        int targetCount = worldGenerator.grassExpansions * resourceExpansionRatio;

        while (currentResourceCount < targetCount)
        {
            int randX = Random.Range(tilemapBounds.xMin, tilemapBounds.xMax + 1);
            int randY = Random.Range(tilemapBounds.yMin, tilemapBounds.yMax + 1);
            Vector3Int basePos = new Vector3Int(randX, randY, 0);

            if (worldGenerator.startingBounds.Contains(basePos))
            {
                yield return null;
                continue;
            }

            int randIndex = PickRandomResourceIndex();
            bool isResourceValid = true;

            foreach (Vector2 pos in possibleResources[randIndex].GetComponent<Resource>().tilesTaken)
            {
                Vector3Int newPos = basePos + new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), 0);
                TileBase tile = tileManager.map.GetTile(newPos);

                if (tile == null || !worldGenerator.possibleTiles.Contains(tile) ||
                    tileManager.tileDataDict[new Vector2(basePos.x, basePos.y) + pos].isOccupied ||
                    tileManager.tileDataDict[new Vector2(basePos.x, basePos.y) + pos].isWaterTile)
                {
                    isResourceValid = false;
                    break;
                }
            }

            if (isResourceValid)
            {
                GameObject resourceClone = Instantiate(possibleResources[randIndex], basePos, Quaternion.identity);
                resourceClone.GetComponent<NetworkObject>().Spawn();
                resourceClone.transform.parent = transform;

                foreach (Vector2 pos in possibleResources[randIndex].GetComponent<Resource>().tilesTaken)
                {
                    tileManager.tileDataDict[new Vector2(basePos.x, basePos.y) + pos].isOccupied = true;
                }
               
                currentResourceCount++;
            }
            yield return null;
        }

        spawningFinished = true;
    }

    private int PickRandomResourceIndex()
    {
        List<int> randomIntWeight = new List<int>();
        for (int i = 0; i < possibleResources.Count; i++) for (int j = 0; j < possibleResources[i].GetComponent<Resource>().rarity; j++) randomIntWeight.Add(i);
        return randomIntWeight[Random.Range(0, randomIntWeight.Count)];
    }
}
