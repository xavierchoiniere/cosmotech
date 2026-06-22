using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Slot : MonoBehaviour
{
    public int itemId = 0;
    public int quantity;
    public List<int> possibleItemIds;
    public bool isChangedByUI;

    void Update()
    {
        if (itemId == 0 && quantity != 0) quantity = 0;
        UIItem uiItem = GetComponentInChildren<UIItem>();
        if (uiItem != null && uiItem.itemAmount != null && transform.parent.CompareTag("Slot Holder"))
        {
            if (itemId > 599 || itemId < 500) quantity = int.Parse(uiItem.itemAmount.text);
            else quantity = Mathf.RoundToInt(transform.GetComponentInChildren<UniqueToolDurability>().currentDurability);
        }
        if (transform.childCount == 0)
        {
            quantity = 0;
            itemId = 0;
        }
    }
}
