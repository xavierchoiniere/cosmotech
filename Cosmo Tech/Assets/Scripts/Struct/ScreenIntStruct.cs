using UnityEngine;

public class ScreenIntStruct : InteractableStructure
{
    public bool isScreenOn;
    public GameObject screenPrefab;
    public GameObject screenTemp;

    override protected void Update()
    {
        base.Update();
        if (isScreenOn)
        {
            player.GetComponent<PlayerMovement>().isInteractingWithUI = isScreenOn;
            player.GetComponent<PlayerTool>().isInteractingWithUI = isScreenOn;
        }
    }

    override protected void DoInteractiveAction()
    {
        SwitchInventoryState();
    }

    public void SwitchInventoryState()
    {
        isScreenOn = !isScreenOn;
        FlipPlayerInventory(isScreenOn);
        if (isScreenOn)
        {
            screenTemp = Instantiate(screenPrefab, GameObject.FindGameObjectWithTag("Canvas").transform, false);
            player.GetComponent<PlayerSpriteManager>().structManager.currentScreenStruct = this;
        }
        else Destroy(screenTemp);
        player.GetComponent<PlayerMovement>().isInteractingWithUI = isScreenOn;
        player.GetComponent<PlayerTool>().isInteractingWithUI = isScreenOn;
    }

    private void FlipPlayerInventory(bool isScreenOpen)
    {
        Transform slotHolder = GameObject.FindGameObjectWithTag("Slot Holder").transform;
        if (isScreenOpen)
        {
            slotHolder.localRotation = Quaternion.Euler(0, 0, -90f);
            slotHolder.localPosition = new Vector2(-810f, 810f);
            foreach (Transform slot in slotHolder.transform)
            {
                slot.localRotation = Quaternion.Euler(0, 0, 90f);
            }
        }
        else
        {
            slotHolder.localRotation = Quaternion.Euler(0, 0, 0);
            slotHolder.localPosition = new Vector2(-810f, -415f);
            foreach (Transform slot in slotHolder.transform)
            {
                slot.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}
