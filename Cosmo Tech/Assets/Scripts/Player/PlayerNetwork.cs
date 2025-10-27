using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;

public class PlayerNetwork : NetworkBehaviour
{
    private PlayerSpriteManager spriteManager;
    private PlayerInventory inventory;
    private Transform managerHolder;

    public GameObject structManagerPrefab;
    public GameObject backpackManager;
    public GameObject slotHolder;
    public GameObject cursor;
    public CinemachineVirtualCamera cinemachineVirtualCamera;

    public StructManager indivStructManager;

    private NetworkVariable<Vector3> netPos = new(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector3> netScale = new(writePerm: NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            managerHolder = GameObject.FindGameObjectWithTag("Manager Holder").GetComponent<Transform>();
            spriteManager = GetComponent<PlayerSpriteManager>();
            inventory = GetComponent<PlayerInventory>();

            GameObject structManagerClone = Instantiate(structManagerPrefab, managerHolder);
            GameObject backpackManagerClone = Instantiate(backpackManager, managerHolder);
            GameObject slotHolderClone = Instantiate(slotHolder, GameObject.FindGameObjectWithTag("Canvas").transform);
            GameObject cursorClone = Instantiate(cursor);
            cursorClone.GetComponent<CustomCursor>().respectivePlayer = gameObject;

            backpackManagerClone.GetComponent<BackpackManager>().slotHolder = slotHolderClone.transform;

            indivStructManager = structManagerClone.GetComponent<StructManager>();
            indivStructManager.tileManager = GameObject.FindGameObjectWithTag("Tile Manager").GetComponent<TileManager>();
            indivStructManager.cursor = cursorClone.transform;
            indivStructManager.respectivePlayer = gameObject;

            spriteManager.structManager = indivStructManager;
            inventory.backpackManager = backpackManagerClone.GetComponent<BackpackManager>();
            GetComponent<PlayerTool>().cursor = cursorClone.transform;

            cinemachineVirtualCamera = GameObject.FindGameObjectWithTag("Virtual Cam").GetComponent<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Follow = transform;
        }
        else
        {
            GameObject structManagerClone = Instantiate(structManagerPrefab, managerHolder);
            indivStructManager = structManagerClone.GetComponent<StructManager>();
            Destroy(GetComponent<PlayerInventory>());
            Destroy(GetComponent<PlayerSpriteManager>());
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            netPos.Value = transform.position;
            netScale.Value = transform.localScale;
        }
        else
        {
            transform.position = netPos.Value;
            transform.localScale = netScale.Value;
        }
    }
}
