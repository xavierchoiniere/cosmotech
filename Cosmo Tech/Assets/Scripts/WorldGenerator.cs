
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class WorldGenerator : MonoBehaviour
{
    public List<TileBase> possibleTiles;
    public int grassExpansions;
    public List<Tilemap> possibleGrassPatches;

    public Tilemap map;

    public BoundsInt startingBounds;
    [SerializeField] private bool placedRequiredWaterPatch;
    private bool isLastPatchWater;

    public void GenerateWorld()
    {
        placedRequiredWaterPatch = false;
        startingBounds = map.cellBounds;
        for (int i = 0; i < grassExpansions; i++)
        {
            BoundsInt newBounds = new() { xMin = startingBounds.xMin - i * 2, yMin = startingBounds.yMin - i * 2, xMax = startingBounds.xMax + i * 2, yMax = startingBounds.yMax + i * 2 };
            GrassTileLoop(newBounds);
        }
        ApplyGrassPatches();
    }

    private void ApplyGrassPatches()
    {
        List<TileBase> basicGrassTiles = new() { possibleTiles[0], possibleTiles[1], possibleTiles[4], possibleTiles[5] };
        BoundsInt expandedBounds = map.cellBounds;
        for (int x = expandedBounds.xMin; x < expandedBounds.xMax; x++)
        {
            for (int y = expandedBounds.yMin; y < expandedBounds.yMax; y++)
            {
                Vector3Int basePos = new Vector3Int(x, y, 0);
                int patchIndex = PickRandomGrassPatch();
                if (Random.Range(0, 25) != 0) continue;
                bool isPatchValid = true;
                for (int xPatch = possibleGrassPatches[patchIndex].cellBounds.xMin; xPatch < possibleGrassPatches[patchIndex].cellBounds.xMax; xPatch++)
                {
                    for (int yPatch = possibleGrassPatches[patchIndex].cellBounds.yMin; yPatch < possibleGrassPatches[patchIndex].cellBounds.yMax; yPatch++)
                    {
                        Vector3Int patchPos = new Vector3Int(xPatch, yPatch, 0);
                        Vector3Int tilePos = basePos + patchPos;
                        TileBase tile = map.GetTile(tilePos);
                        if (tile == null || !expandedBounds.Contains(tilePos) || startingBounds.Contains(tilePos) || !basicGrassTiles.Contains(tile))
                        {
                            isPatchValid = false;
                            break;
                        }

                    }
                }
                if (isPatchValid)
                {
                    for (int xPatch = possibleGrassPatches[patchIndex].cellBounds.xMin; xPatch < possibleGrassPatches[patchIndex].cellBounds.xMax; xPatch++)
                    {
                        for (int yPatch = possibleGrassPatches[patchIndex].cellBounds.yMin; yPatch < possibleGrassPatches[patchIndex].cellBounds.yMax; yPatch++)
                        {
                            Vector3Int patchPos = new Vector3Int(xPatch, yPatch, 0);
                            Vector3Int tilePos = basePos + patchPos;

                            TileBase wantedTile = possibleGrassPatches[patchIndex].GetTile(patchPos);
                            Matrix4x4 transform = possibleGrassPatches[patchIndex].GetTransformMatrix(patchPos);

                            map.SetTile(tilePos, wantedTile);
                            map.SetTransformMatrix(tilePos, transform);
                        }
                    }
                    if (isLastPatchWater)
                    {
                        isLastPatchWater = false;
                        placedRequiredWaterPatch = true;
                    }
                }
            }
        }
    }

    private void GrassTileLoop(BoundsInt bounds)
    {
        for (int i = 0; i <= bounds.xMax; i++)
        {
            int x = bounds.xMin + i * 2;
            SetGrassTiles(new Vector3Int(x, bounds.yMin, 0));
            SetGrassTiles(new Vector3Int(x, bounds.yMax, 0));
        }

        for (int i = 1; i <= bounds.yMax; i++)
        {
            int y = bounds.yMin + i * 2;
            SetGrassTiles(new Vector3Int(bounds.xMin, y, 0));
            SetGrassTiles(new Vector3Int(bounds.xMax, y, 0));
        }
    }

    private void SetGrassTiles(Vector3Int pos)
    {
        map.SetTile(pos, possibleTiles[4]);
        map.SetTile(new Vector3Int(pos.x + 1, pos.y, 0), possibleTiles[5]);
        map.SetTile(new Vector3Int(pos.x + 1, pos.y + 1, 0), possibleTiles[1]);
        map.SetTile(new Vector3Int(pos.x, pos.y + 1, 0), possibleTiles[0]);
    }

    private int PickRandomGrassPatch()
    {
        if (!placedRequiredWaterPatch)
        {
            isLastPatchWater = true;
            return Random.Range(7, possibleGrassPatches.Count);
        }
        List<int> increasedIntChances = new List<int>();
        for (int i = 0; i < possibleGrassPatches.Count; i++)
        {
            if (i >= 5) increasedIntChances.Add(i);
            else
            {
               for (int j = 0; j < 7; j++) increasedIntChances.Add(i);
            }
        }
        return increasedIntChances[Random.Range(0, increasedIntChances.Count)];
    }
}
