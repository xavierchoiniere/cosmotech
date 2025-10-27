using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetStructManager : NetworkBehaviour
{
    public List<GameObject> possibleStructures;

    public void CreateStructureOnNetwork(string structName, Vector2 positionToPlace)
    {
        if (!IsServer) return;
        GameObject wantedStruct = possibleStructures.Find(x => x.name == structName);
        GameObject clone = Instantiate(wantedStruct, positionToPlace, Quaternion.identity);
        clone.GetComponent<NetworkObject>().Spawn();
        clone.transform.parent = GameObject.FindGameObjectWithTag("Global Struct Manager").transform;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestStructureSpawnServerRpc(string structureName, Vector2 position)
    {
        CreateStructureOnNetwork(structureName, position);
    }
}
