
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class DoubleResourceData
{
    public int firstResourceId;
    public int secondResourceId;
    public float timeToProcess;
    public int productId;
}

public struct DoubleResourceSyncData : INetworkSerializable
{
    public int fuelId;
    public int fuelQuantity;

    public int firstResourceId;
    public int firstResQuantity;

    public int secondResourceId;
    public int secondResQuantity;

    public int productId;
    public int prodQuantity;

    public float fuelSliderValue;
    public float arrowSliderValue;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref fuelId);
        serializer.SerializeValue(ref fuelQuantity);

        serializer.SerializeValue(ref firstResourceId);
        serializer.SerializeValue(ref firstResQuantity);

        serializer.SerializeValue(ref secondResourceId);
        serializer.SerializeValue(ref secondResQuantity);

        serializer.SerializeValue(ref productId);
        serializer.SerializeValue(ref prodQuantity);

        serializer.SerializeValue(ref fuelSliderValue);
        serializer.SerializeValue(ref arrowSliderValue);
    }
}


public class DoubleResourceFuelBarManager : NetworkBehaviour
{
    public int fuelId;
    public int firstResourceId;
    public int secondResourceId;
    public int productId;

    public int fuelQuantity;
    public int firstResQuantity;
    public int secondResQuantity;
    public int prodQuantity;

    public List<FuelData> fuelDataList;
    public List<DoubleResourceData> resourceDataList;

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

        if (firstResourceId != 0 && secondResourceId != 0 && !isProcessing)
        {
            DoubleResourceData matchingResource = resourceDataList.FirstOrDefault(r =>
            (r.firstResourceId == firstResourceId && r.secondResourceId == secondResourceId) ||
            (r.firstResourceId == secondResourceId && r.secondResourceId == firstResourceId)); //checking if slot 1 has 1st resource and slot 2 has 2nd resource or vice versa, as long as it has both in any order
            if (matchingResource != null && Mathf.RoundToInt(fuelSliderValue) >= matchingResource.timeToProcess * 10f) //see SingleResourceFuelBarManager for explanation of 10f
            {
                if (productId != 0 && productId != matchingResource.productId) return;
                firstResQuantity--;
                secondResQuantity--;
                processTimer = matchingResource.timeToProcess;
                totalProcessTime = matchingResource.timeToProcess;
                wantedResultId = matchingResource.productId;
                isProcessing = true;
                if (firstResQuantity == 0) firstResourceId = 0;
                if (secondResQuantity == 0) secondResourceId = 0;
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
        DoubleResourceSyncData syncData = new()
        {
            fuelId = fuelId,
            fuelQuantity = fuelQuantity,
            firstResourceId = firstResourceId,
            firstResQuantity = firstResQuantity,
            secondResourceId = secondResourceId,
            secondResQuantity = secondResQuantity,
            productId = productId,
            prodQuantity = prodQuantity,
            fuelSliderValue = fuelSliderValue,
            arrowSliderValue = arrowSliderValue
        };
        UpdateAllDataClientRPC(syncData);
    }

    [ClientRpc]
    private void UpdateAllDataClientRPC(DoubleResourceSyncData data)
    {
        fuelId = data.fuelId;
        fuelQuantity = data.fuelQuantity;

        firstResourceId = data.firstResourceId;
        firstResQuantity = data.firstResQuantity;

        secondResourceId = data.secondResourceId;
        secondResQuantity = data.secondResQuantity;

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
    public void SetFirstResourceSlotServerRpc(int itemId, int quantity)
    {
        firstResourceId = itemId;
        firstResQuantity = quantity;
        UpdateData();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void SetSecondResourceSlotServerRpc(int itemId, int quantity)
    {
        secondResourceId = itemId;
        secondResQuantity = quantity;
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
