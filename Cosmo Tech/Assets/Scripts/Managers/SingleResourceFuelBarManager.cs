
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class FuelData
{
    public int fuelId;
    public int fuelValue;
}

[System.Serializable]
public class SingleResourceData
{
    public int resourceId;
    public float timeToProcess;
    public int productId;
}

public struct SingleResourceSyncData: INetworkSerializable
{
    public int fuelId;
    public int fuelQuantity;
    
    public int resourceId;
    public int resQuantity;

    public int productId;
    public int prodQuantity;

    public float fuelSliderValue;
    public float arrowSliderValue;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref fuelId);
        serializer.SerializeValue(ref fuelQuantity);

        serializer.SerializeValue(ref resourceId);
        serializer.SerializeValue(ref resQuantity);

        serializer.SerializeValue(ref productId);
        serializer.SerializeValue(ref prodQuantity);

        serializer.SerializeValue(ref fuelSliderValue);
        serializer.SerializeValue(ref arrowSliderValue);
    }
}


public class SingleResourceFuelBarManager : NetworkBehaviour
{
    public int fuelId;
    public int resourceId;
    public int productId;

    public int fuelQuantity;
    public int resQuantity;
    public int prodQuantity;

    public List<FuelData> fuelDataList;
    public List<SingleResourceData> resourceDataList;

    public float fuelSliderValue;
    public float fuelSliderMaxValue;

    public float arrowSliderValue;
    public float arrowSliderMaxValue;

    private bool isProcessing;
    private float processTimer;
    private float totalProcessTime;
    private int wantedResultId;

    void Update()
    {
        if (!IsServer) return;
        
        if (fuelId != 0 && !isProcessing)
        {
            FuelData matchingFuel = fuelDataList.FirstOrDefault(f => f.fuelId == fuelId);
            if (matchingFuel != null && fuelSliderValue < fuelSliderMaxValue)
            {
                float fuelUntilMax = fuelSliderMaxValue - fuelSliderValue;
                fuelSliderValue += Mathf.Min(matchingFuel.fuelValue, fuelUntilMax);
                fuelQuantity--;
                if (fuelQuantity == 0) fuelId = 0;
                UpdateData();
            }
        }

        if (resourceId != 0 && !isProcessing)
        {
            SingleResourceData matchingResource = resourceDataList.FirstOrDefault(r => r.resourceId == resourceId);
            if (matchingResource != null && Mathf.RoundToInt(fuelSliderValue) >= matchingResource.timeToProcess * 10f) //10 is fuel efficiency (10 fuel = 1 second of processing) could be replaced by variable when furnace upgrades are added
            {
                if (productId != 0 && productId != matchingResource.productId) return;
                resQuantity--;
                processTimer = matchingResource.timeToProcess;
                totalProcessTime = matchingResource.timeToProcess;
                wantedResultId = matchingResource.productId;
                isProcessing = true;
                if (resQuantity == 0) resourceId = 0;
                UpdateData();
            }
        }

        if (isProcessing && processTimer > 0 && fuelSliderValue > 0)
        {
            processTimer -= Time.deltaTime;
            fuelSliderValue -= Time.deltaTime * 10f;
            arrowSliderValue += Time.deltaTime * (arrowSliderMaxValue / totalProcessTime);
            UpdateData();
        }
        if (isProcessing && processTimer <= 0)
        {
            isProcessing = false;
            processTimer = 0;
            totalProcessTime = 0;
            arrowSliderValue = 0;
            fuelSliderValue = Mathf.Round(fuelSliderValue);
            if (productId == 0)
            {
                productId = wantedResultId;
                prodQuantity++;
            }
            else if (productId == wantedResultId) prodQuantity++;
            UpdateData();
        }
    }

    private void UpdateData()
    {
        SingleResourceSyncData syncData = new()
        {
            fuelId = fuelId,  
            fuelQuantity = fuelQuantity,
            resourceId = resourceId,
            resQuantity = resQuantity,
            productId = productId,
            prodQuantity = prodQuantity,
            fuelSliderValue = fuelSliderValue,
            arrowSliderValue = arrowSliderValue
        };
        UpdateAllDataClientRPC(syncData);
    }

    [ClientRpc]
    private void UpdateAllDataClientRPC(SingleResourceSyncData data)
    {
        fuelId = data.fuelId;
        fuelQuantity = data.fuelQuantity;
        
        resourceId = data.resourceId;
        resQuantity = data.resQuantity;

        productId = data.productId;
        prodQuantity = data.prodQuantity;

        fuelSliderValue = data.fuelSliderValue;
        arrowSliderValue = data.arrowSliderValue;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void SetFuelSlotServerRpc(int itemId, int quantity)
    {
        fuelId = itemId;
        fuelQuantity = quantity;
        UpdateData();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void SetResourceSlotServerRpc(int itemId, int quantity)
    {
        resourceId = itemId;
        resQuantity = quantity;
        UpdateData();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void SetProductSlotServerRpc(int itemId, int quantity)
    {
        productId = itemId;
        prodQuantity = quantity;
        UpdateData();
    }

}
