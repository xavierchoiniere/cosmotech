using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class ToolVisualManager : NetworkBehaviour
{
    public List<GameObject> possibleTools;
    private int currentToolID;
    private NetworkVariable<int> currentToolNetworkID = new(writePerm: NetworkVariableWritePermission.Owner);
    private bool toolInstantiatedOnSpawn;

    void Start()
    {
        currentToolNetworkID.OnValueChanged += HandleToolChange;
        toolInstantiatedOnSpawn = false;
    }

    public override void OnDestroy()
    {
        currentToolNetworkID.OnValueChanged -= HandleToolChange;
    }

    void Update()
    {
        if (IsOwner)
        {
            HandleToolVisuals();
            currentToolNetworkID.Value = currentToolID;
        }
        if (!IsOwner && currentToolNetworkID.Value != 0 && !toolInstantiatedOnSpawn)
        {
            HandleToolChange(0, currentToolNetworkID.Value);
            toolInstantiatedOnSpawn = true;
        }
    }

    private void HandleToolVisuals()
    {
        if (transform.childCount == 0 && GameObject.FindGameObjectWithTag("Tool Slot").transform.childCount > 0)
        {
            GameObject wantedTool = possibleTools.Find(x =>
            x.GetComponent<ToolStats>().toolID == GameObject.FindGameObjectWithTag("Tool Slot").transform.GetChild(0).GetComponent<UIItem>().itemID);
            Instantiate(wantedTool, transform, false);
            currentToolID = wantedTool.GetComponent<ToolStats>().toolID;
        }
        if (transform.childCount == 1 && GameObject.FindGameObjectWithTag("Tool Slot").transform.childCount > 0)
        {
            if (GameObject.FindGameObjectWithTag("Tool Slot").transform.GetChild(0).GetComponent<UIItem>().itemID != currentToolID) Destroy(transform.GetChild(0).gameObject);
        }
        if (transform.childCount == 1 && GameObject.FindGameObjectWithTag("Tool Slot").transform.childCount == 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }
    private void HandleToolChange(int previousValue, int newValue)
    {
        if (!IsOwner)
        {
            GameObject wantedTool = possibleTools.Find(x => x.GetComponent<ToolStats>().toolID == newValue);
            if (transform.childCount > 0)
            {
                Destroy(transform.GetChild(0).gameObject);
            }
            Instantiate(wantedTool, transform, false);
        }
    }
}
