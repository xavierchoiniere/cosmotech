
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
public class ResourceData
{
    public int resourceId;
    public float timeToProcess;
    public int productId;
}

public struct SyncData: INetworkSerializable
{
    public int currentFuelId;
    public int currentFuelQuantity;
    
    public int currentResourceId;
    public int currentResQuantity;

    public int currentProductId;
    public int currentProdQuantity;

    public float fuelSliderValue;
    public float arrowSliderValue;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref currentFuelId);
        serializer.SerializeValue(ref currentFuelQuantity);

        serializer.SerializeValue(ref currentResourceId);
        serializer.SerializeValue(ref currentResQuantity);

        serializer.SerializeValue(ref currentProductId);
        serializer.SerializeValue(ref currentProdQuantity);

        serializer.SerializeValue(ref fuelSliderValue);
        serializer.SerializeValue(ref arrowSliderValue);
    }
}


public class SingleResourceFuelBarManager : NetworkBehaviour
{
    public int currentFuelId;
    public int currentResourceId;
    public int currentProductId;

    public int currentFuelQuantity;
    public int currentResQuantity;
    public int currentProdQuantity;

    public List<FuelData> fuelDataList;
    public List<ResourceData> resourceDataList;

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
        
        if (currentFuelId != 0 && !isProcessing)
        {
            FuelData matchingFuel = fuelDataList.FirstOrDefault(f => f.fuelId == currentFuelId);
            if (matchingFuel != null && fuelSliderValue < fuelSliderMaxValue)
            {
                float fuelUntilMax = fuelSliderMaxValue - fuelSliderValue;
                fuelSliderValue += Mathf.Min(matchingFuel.fuelValue, fuelUntilMax);
                currentFuelQuantity--;
                if (currentFuelQuantity == 0) currentFuelId = 0;
                UpdateData();
            }
        }

        if (currentResourceId != 0 && !isProcessing)
        {
            ResourceData matchingResource = resourceDataList.FirstOrDefault(r => r.resourceId == currentResourceId);
            if (matchingResource != null && Mathf.RoundToInt(fuelSliderValue) >= matchingResource.timeToProcess * 10f)
            {
                if (currentProductId != 0 && currentProductId != matchingResource.productId) return;
                currentResQuantity--;
                processTimer = matchingResource.timeToProcess;
                totalProcessTime = matchingResource.timeToProcess;
                wantedResultId = matchingResource.productId;
                isProcessing = true;
                if (currentResQuantity == 0) currentResourceId = 0;
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
            if (currentProductId == 0)
            {
                currentProductId = wantedResultId;
                currentProdQuantity++;
            }
            else if (currentProductId == wantedResultId) currentProdQuantity++;
            UpdateData();
        }
    }

    private void UpdateData()
    {
        SyncData syncData = new()
        {
            currentFuelId = currentFuelId,
            currentFuelQuantity = currentFuelQuantity,
            currentResourceId = currentResourceId,
            currentResQuantity = currentResQuantity,
            currentProductId = currentProductId,
            currentProdQuantity = currentProdQuantity,
            fuelSliderValue = fuelSliderValue,
            arrowSliderValue = arrowSliderValue
        };
        UpdateAllDataClientRPC(syncData);
    }

    [ClientRpc]
    private void UpdateAllDataClientRPC(SyncData data)
    {
        currentFuelId = data.currentFuelId;
        currentFuelQuantity = data.currentFuelQuantity;
        
        currentResourceId = data.currentResourceId;
        currentResQuantity = data.currentResQuantity;

        currentProductId = data.currentProductId;
        currentProdQuantity = data.currentProdQuantity;

        fuelSliderValue = data.fuelSliderValue;
        arrowSliderValue = data.arrowSliderValue;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetFuelSlotServerRpc(int itemId, int quantity)
    {
        currentFuelId = itemId;
        currentFuelQuantity = quantity;
        UpdateData();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetResourceSlotServerRpc(int itemId, int quantity)
    {
        currentResourceId = itemId;
        currentResQuantity = quantity;
        UpdateData();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetProductSlotServerRpc(int itemId, int quantity)
    {
        currentProductId = itemId;
        currentProdQuantity = quantity;
        UpdateData();
    }

}
