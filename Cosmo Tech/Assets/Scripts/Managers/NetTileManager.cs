using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct NetTileData : INetworkSerializable, IEquatable<NetTileData>
{
    public Vector2 position;
    public bool isOccupied;
    public bool isWaterTile;
    public bool isShowingTile;
    public FixedString64Bytes tileName;
    public float transformMatrix0;
    public float transformMatrix1;
    public float transformMatrix4;
    public float transformMatrix5;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref isOccupied);
        serializer.SerializeValue(ref isWaterTile);
        serializer.SerializeValue(ref isShowingTile);
        serializer.SerializeValue(ref tileName);
        serializer.SerializeValue(ref transformMatrix0);
        serializer.SerializeValue(ref transformMatrix1);
        serializer.SerializeValue(ref transformMatrix4);
        serializer.SerializeValue(ref transformMatrix5);
    }

    public bool Equals(NetTileData other)
    {
        return position == other.position &&
               isOccupied == other.isOccupied &&
               isWaterTile == other.isWaterTile &&
               isShowingTile == other.isShowingTile &&
               tileName.Equals(other.tileName) &&
               transformMatrix0.Equals(other.transformMatrix0) &&
               transformMatrix1.Equals(other.transformMatrix0) &&
               transformMatrix4.Equals(other.transformMatrix0) &&
               transformMatrix5.Equals(other.transformMatrix5);
    }
}
public class NetTileManager : NetworkBehaviour
{
    public Dictionary<Vector2, TileData> tileDataDictToCopy;
    private NetworkList<NetTileData> tileDataList = new();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost)
        {
            tileDataList.Clear();
            foreach (var tile in tileDataDictToCopy)
            {
                tileDataList.Add(new()
                {
                    position = tile.Key,
                    isOccupied = tile.Value.isOccupied,
                    isWaterTile = tile.Value.isWaterTile,
                    isShowingTile = tile.Value.isShowingTile,
                    tileName = tile.Value.tileName,
                    transformMatrix0 = GameObject.FindGameObjectWithTag("Tile Manager").GetComponent<TileManager>().map.GetTransformMatrix(
                        new Vector3Int(Mathf.RoundToInt(tile.Key.x), Mathf.RoundToInt(tile.Key.y), 0))[0,0],
                    transformMatrix1 = GameObject.FindGameObjectWithTag("Tile Manager").GetComponent<TileManager>().map.GetTransformMatrix(
                        new Vector3Int(Mathf.RoundToInt(tile.Key.x), Mathf.RoundToInt(tile.Key.y), 0))[0, 1],
                    transformMatrix4 = GameObject.FindGameObjectWithTag("Tile Manager").GetComponent<TileManager>().map.GetTransformMatrix(
                        new Vector3Int(Mathf.RoundToInt(tile.Key.x), Mathf.RoundToInt(tile.Key.y), 0))[1, 0],
                    transformMatrix5 = GameObject.FindGameObjectWithTag("Tile Manager").GetComponent<TileManager>().map.GetTransformMatrix(
                        new Vector3Int(Mathf.RoundToInt(tile.Key.x), Mathf.RoundToInt(tile.Key.y), 0))[1, 1],
                });
            }
            return;
        }
        GameObject.FindGameObjectWithTag("Tile Manager").GetComponent<TileManager>().CopyTilesFromHost(tileDataList);
    }
}
