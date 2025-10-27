using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using static UnityEditor.PlayerSettings;

public class TileManager : NetworkBehaviour
{
    public Tilemap map;
    [SerializeField] private GameObject tileObject;
    [SerializeField] private GameObject waterTileCollider;
    [SerializeField] private Transform tileHolder;

    public WorldGenerator worldGenerator;
    public bool isShowingAllTiles;
    public Dictionary<Vector2, TileData> tileDataDict;

    [SerializeField] private List<Vector2> rocketTilePositions;
    public bool tilesInitiated;
    public GameObject netTileManagerPrefab;

    void Start()
    {
        tileDataDict = new Dictionary<Vector2, TileData>();
    }

    public void GenerateTiles()
    {
        InitiateTiles();
        ManageRocketTiles();
        GameObject netTileManagerClone = Instantiate(netTileManagerPrefab);
        netTileManagerClone.GetComponent<NetTileManager>().tileDataDictToCopy = tileDataDict;
        netTileManagerClone.GetComponent<NetworkObject>().Spawn();
        netTileManagerClone.transform.parent = GameObject.FindGameObjectWithTag("Manager Holder").transform;
        tilesInitiated = true;
    }

    public void CopyTilesFromHost(NetworkList<NetTileData> netTileDataList)
    {
        foreach (var netTileData in netTileDataList)
        {
            Vector3Int tilePos = new Vector3Int(Mathf.RoundToInt(netTileData.position.x), Mathf.RoundToInt(netTileData.position.y), 0);

            map.SetTile(tilePos, worldGenerator.possibleTiles.Find(x => x.name == netTileData.tileName.ToString()));
            Matrix4x4 newTransformMatrix = map.GetTransformMatrix(tilePos);
            if (netTileData.transformMatrix1 == 0 && netTileData.transformMatrix4 == 0) newTransformMatrix = Matrix4x4.Scale(new Vector3(netTileData.transformMatrix0, netTileData.transformMatrix5));
            else
            {
                float angle = 0;
                if (netTileData.transformMatrix1 == -1 && netTileData.transformMatrix4 == 1) angle = 90;
                if (netTileData.transformMatrix1 == 1 && netTileData.transformMatrix4 == -1) angle = 270;
                newTransformMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, angle));
            }
            map.SetTransformMatrix(tilePos, newTransformMatrix);

            GameObject tileClone = Instantiate(tileObject, netTileData.position, Quaternion.identity);
            tileClone.transform.parent = tileHolder;
            TileData tileData = tileClone.GetComponent<TileData>();
            tileData.isOccupied = netTileData.isOccupied;
            tileData.isShowingTile = netTileData.isShowingTile;
            tileData.isWaterTile = netTileData.isWaterTile;
            if (tileData.isWaterTile) Instantiate(waterTileCollider, tileClone.transform);
            tileData.tileName = netTileData.tileName.ToString();
            tileDataDict[netTileData.position] = tileData;
        }
    }

    public void SwitchShowTiles()
    {
        isShowingAllTiles = !isShowingAllTiles;
        foreach (var tileData in tileDataDict.Values) tileData.isShowingTile = isShowingAllTiles;
    }

    public bool AreAllTilesFree(List<Vector2> tilePositions)
    {
        bool areTilesFree = true;
        foreach (var tilePos in tilePositions)
        {
            if (tileDataDict[tilePos].isOccupied) areTilesFree = false;
        }
        return areTilesFree;
    }

    public void ManageGivenTiles(List<Vector2> tilePosArray, bool tileOccupation)
    {
        foreach (Vector2 pos in tilePosArray)
        {
            TileData tileData = tileDataDict[pos];
            tileData.isOccupied = tileOccupation;
        }
    }
    
    private void InitiateTiles()
    {
        BoundsInt bounds = map.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++) CreateTileObject(new Vector3Int(x, y, 0));
        }
    }
    private void CreateTileObject(Vector3Int pos)
    {
        TileBase tile = map.GetTile(pos);
        if (tile != null)
        {
            GameObject tileClone = Instantiate(tileObject, pos, Quaternion.identity);
            tileClone.transform.parent = tileHolder;
            TileData tileData = tileClone.GetComponent<TileData>();
            tileData.isOccupied = false;
            tileData.isShowingTile = false;
            tileData.isWaterTile = false;
            if (tile.name.ToLower().Contains("water"))
            {
                tileData.isWaterTile = true;
                Instantiate(waterTileCollider, tileClone.transform);
            }
            tileData.tileName = tile.name;
            tileDataDict[new Vector2(pos.x, pos.y)] = tileData;
        }
    }
    private void ManageRocketTiles()
    {
        foreach (Vector2 pos in rocketTilePositions)
        {
            TileData tileData = tileDataDict[pos];
            tileData.isOccupied = true;
        }
    }
}
