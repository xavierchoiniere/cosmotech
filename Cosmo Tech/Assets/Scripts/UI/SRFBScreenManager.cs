
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
        if (srfbManager.fuelId != 0)
        {
            fuelSlot.itemId = srfbManager.fuelId;
            fuelSlot.quantity = srfbManager.fuelQuantity;
            GameObject item = Instantiate(possibleFuels.Find(x => x.GetComponent<UIItem>().itemId == srfbManager.fuelId), fuelSlot.transform);
            Instantiate(itemAmount, item.transform, false);
        }
        if (srfbManager.resourceId != 0)
        {
            resourceSlot.itemId = srfbManager.resourceId;
            resourceSlot.quantity = srfbManager.resQuantity;
            GameObject item = Instantiate(possibleResources.Find(x => x.GetComponent<UIItem>().itemId == srfbManager.resourceId), resourceSlot.transform);
            Instantiate(itemAmount, item.transform, false);
        }
        if (srfbManager.productId != 0)
        {
            productSlot.itemId = srfbManager.productId;
            productSlot.quantity = srfbManager.prodQuantity;
            GameObject item = Instantiate(possibleProducts.Find(x => x.GetComponent<UIItem>().itemId == srfbManager.productId), productSlot.transform);
            Instantiate(itemAmount, item.transform, false);
        }
        if (srfbManager.fuelSliderMaxValue == 0) srfbManager.fuelSliderMaxValue = fuelSlider.maxValue;
        if (srfbManager.arrowSliderMaxValue == 0) srfbManager.arrowSliderMaxValue = arrowSlider.maxValue;
        if (srfbManager.fuelSliderValue != 0) fuelSlider.slider.value = srfbManager.fuelSliderValue;
        if (srfbManager.arrowSliderValue != 0) arrowSlider.slider.value = srfbManager.arrowSliderValue;
    }

    void Update()
    {
        if (srfbManager.fuelId != fuelSlot.itemId || srfbManager.fuelQuantity != fuelSlot.quantity)
        {
            if (fuelSlot.isChangedByUI)
            {
                srfbManager.SetFuelSlotServerRpc(fuelSlot.itemId, fuelSlot.quantity);
                fuelSlot.isChangedByUI = false;
            }
            else
            {
                fuelSlot.itemId = srfbManager.fuelId;
                fuelSlot.quantity = srfbManager.fuelQuantity;
            }
        }
        if (srfbManager.resourceId != resourceSlot.itemId || srfbManager.resQuantity != resourceSlot.quantity)
        {
            if (resourceSlot.isChangedByUI)
            {
                srfbManager.SetResourceSlotServerRpc(resourceSlot.itemId, resourceSlot.quantity);
                resourceSlot.isChangedByUI = false;
            }
            else
            {
                resourceSlot.itemId = srfbManager.resourceId;
                resourceSlot.quantity = srfbManager.resQuantity;
            }
        }
        if (srfbManager.productId != productSlot.itemId || srfbManager.prodQuantity != productSlot.quantity)
        {
            if (productSlot.isChangedByUI)
            {
                srfbManager.SetProductSlotServerRpc(productSlot.itemId, productSlot.quantity);
                productSlot.isChangedByUI = false;
            }
            else
            {
                productSlot.itemId = srfbManager.productId;
                productSlot.quantity = srfbManager.prodQuantity;
                if (productSlot.transform.childCount == 0)
                {
                    GameObject item = Instantiate(possibleProducts.Find(x => x.GetComponent<UIItem>().itemId == srfbManager.productId), productSlot.transform);
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

