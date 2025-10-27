using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TileData : MonoBehaviour
{
    public bool isOccupied;
    public bool isWaterTile;
    public bool isShowingTile;
    public string tileName;

    void Update()
    {
        if (isShowingTile)
        {
            if (isOccupied && !transform.GetChild(1).gameObject.activeInHierarchy)
            {
                transform.GetChild(1).gameObject.SetActive(true);
                transform.GetChild(0).gameObject.SetActive(false);
            }
            if (!isOccupied && !transform.GetChild(0).gameObject.activeInHierarchy)
            {
                transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
