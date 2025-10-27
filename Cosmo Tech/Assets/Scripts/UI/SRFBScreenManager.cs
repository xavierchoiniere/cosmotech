
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;



public class SingleResourceFuelBarScreenManager : MonoBehaviour
{
    public GameObject itemAmount;
    public SingleResourceFuelBarManager srfbManager;

    public Slot fuelSlot;
    public Slot resourceSlot;
    public Slot productSlot;

    public SliderManager fuelSlider;
    public SliderManager arrowSlider;

    public List<GameObject> possibleFuels;
    public List<GameObject> possibleResources; 
    public List<GameObject> possibleProducts;

    void Start()
    {
        Transform globalStructManager = GameObject.FindGameObjectWithTag("Global Struct Manager").transform;
        foreach (Transform child in globalStructManager)
        {
            if (child.GetComponent<ScreenIntStruct>() != null) 
            {
                if (child.GetComponent<ScreenIntStruct>().screenTemp == gameObject)
                {
                    srfbManager = child.GetComponent<SingleResourceFuelBarManager>();
                    break;
                }
            }
        }
        if (srfbManager.currentFuelId != 0)
        {
            fuelSlot.currentItemId = srfbManager.currentFuelId;
            fuelSlot.quantity = srfbManager.currentFuelQuantity;
            GameObject item = Instantiate(possibleFuels.Find(x => x.GetComponent<UIItem>().itemID == srfbManager.currentFuelId), fuelSlot.transform);
            Instantiate(itemAmount, item.transform, false);
        }
        if (srfbManager.currentResourceId != 0)
        {
            resourceSlot.currentItemId = srfbManager.currentResourceId;
            resourceSlot.quantity = srfbManager.currentResQuantity;
            GameObject item = Instantiate(possibleResources.Find(x => x.GetComponent<UIItem>().itemID == srfbManager.currentResourceId), resourceSlot.transform);
            Instantiate(itemAmount, item.transform, false);
        }
        if (srfbManager.currentProductId != 0)
        {
            productSlot.currentItemId = srfbManager.currentProductId;
            productSlot.quantity = srfbManager.currentProdQuantity;
            GameObject item = Instantiate(possibleProducts.Find(x => x.GetComponent<UIItem>().itemID == srfbManager.currentProductId), productSlot.transform);
            Instantiate(itemAmount, item.transform, false);
        }
        if (srfbManager.fuelSliderMaxValue == 0) srfbManager.fuelSliderMaxValue = fuelSlider.maxValue;
        if (srfbManager.arrowSliderMaxValue == 0) srfbManager.arrowSliderMaxValue = arrowSlider.maxValue;
        if (srfbManager.fuelSliderValue != 0) fuelSlider.slider.value = srfbManager.fuelSliderValue;
        if (srfbManager.arrowSliderValue != 0) arrowSlider.slider.value = srfbManager.arrowSliderValue;
    }

    void Update()
    {
        if (srfbManager.currentFuelId != fuelSlot.currentItemId || srfbManager.currentFuelQuantity != fuelSlot.quantity)
        {
            if (fuelSlot.isChangedByUI)
            {
                srfbManager.SetFuelSlotServerRpc(fuelSlot.currentItemId, fuelSlot.quantity);
                fuelSlot.isChangedByUI = false;
            }
            else
            {
                fuelSlot.currentItemId = srfbManager.currentFuelId;
                fuelSlot.quantity = srfbManager.currentFuelQuantity;
            }
        }
        if (srfbManager.currentResourceId != resourceSlot.currentItemId || srfbManager.currentResQuantity != resourceSlot.quantity)
        {
            if (resourceSlot.isChangedByUI)
            {
                srfbManager.SetResourceSlotServerRpc(resourceSlot.currentItemId, resourceSlot.quantity);
                resourceSlot.isChangedByUI = false;
            }
            else
            {
                resourceSlot.currentItemId = srfbManager.currentResourceId;
                resourceSlot.quantity = srfbManager.currentResQuantity;
            }
        }
        if (srfbManager.currentProductId != productSlot.currentItemId || srfbManager.currentProdQuantity != productSlot.quantity)
        {
            if (productSlot.isChangedByUI)
            {
                srfbManager.SetProductSlotServerRpc(productSlot.currentItemId, productSlot.quantity);
                productSlot.isChangedByUI = false;
            }
            else
            {
                productSlot.currentItemId = srfbManager.currentProductId;
                productSlot.quantity = srfbManager.currentProdQuantity;
                if (productSlot.transform.childCount == 0)
                {
                    GameObject item = Instantiate(possibleProducts.Find(x => x.GetComponent<UIItem>().itemID == srfbManager.currentProductId), productSlot.transform);
                    Instantiate(itemAmount, item.transform, false);
                }
               
            }
        }
        fuelSlider.currentValue = srfbManager.fuelSliderValue;
        arrowSlider.currentValue = srfbManager.arrowSliderValue;
        if (fuelSlot.quantity == 0 && fuelSlot.transform.childCount > 0) Destroy(fuelSlot.GetComponentInChildren<UIItem>().gameObject);
        if (resourceSlot.quantity == 0 && resourceSlot.transform.childCount > 0) Destroy(resourceSlot.GetComponentInChildren<UIItem>().gameObject);
        if (productSlot.quantity == 0 && productSlot.transform.childCount > 0) Destroy(productSlot.GetComponentInChildren<UIItem>().gameObject);
    }
}

