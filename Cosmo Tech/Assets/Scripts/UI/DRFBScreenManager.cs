
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;



public class DoubleResourceFuelBarScreenManager : MonoBehaviour
{
    public GameObject itemAmount;
    public DoubleResourceFuelBarManager drfbManager;

    public Slot fuelSlot;
    public Slot resourceSlot1;
    public Slot resourceSlot2;
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
                    drfbManager = child.GetComponent<DoubleResourceFuelBarManager>();
                    break;
                }
            }
        }
        if (drfbManager.fuelId != 0)
        {
            fuelSlot.itemId = drfbManager.fuelId;
            fuelSlot.quantity = drfbManager.fuelQuantity;
            GameObject item = Instantiate(possibleFuels.Find(x => x.GetComponent<UIItem>().itemId == drfbManager.fuelId), fuelSlot.transform);
            Instantiate(itemAmount, item.transform, false);
        }
        if (drfbManager.firstResourceId != 0)
        {
            resourceSlot1.itemId = drfbManager.firstResourceId;
            resourceSlot1.quantity = drfbManager.firstResQuantity;
            GameObject item = Instantiate(possibleResources.Find(x => x.GetComponent<UIItem>().itemId == drfbManager.firstResourceId), resourceSlot1.transform);
            Instantiate(itemAmount, item.transform, false);
        }
        if (drfbManager.secondResourceId != 0)
        {
            resourceSlot2.itemId = drfbManager.secondResourceId;
            resourceSlot2.quantity = drfbManager.secondResQuantity;
            GameObject item = Instantiate(possibleResources.Find(x => x.GetComponent<UIItem>().itemId == drfbManager.secondResourceId), resourceSlot2.transform);
            Instantiate(itemAmount, item.transform, false);
        }
        if (drfbManager.productId != 0)
        {
            productSlot.itemId = drfbManager.productId;
            productSlot.quantity = drfbManager.prodQuantity;
            GameObject item = Instantiate(possibleProducts.Find(x => x.GetComponent<UIItem>().itemId == drfbManager.productId), productSlot.transform);
            Instantiate(itemAmount, item.transform, false);
        }
        if (drfbManager.fuelSliderMaxValue == 0) drfbManager.fuelSliderMaxValue = fuelSlider.maxValue;
        if (drfbManager.arrowSliderMaxValue == 0) drfbManager.arrowSliderMaxValue = arrowSlider.maxValue;
        if (drfbManager.fuelSliderValue != 0) fuelSlider.slider.value = drfbManager.fuelSliderValue;
        if (drfbManager.arrowSliderValue != 0) arrowSlider.slider.value = drfbManager.arrowSliderValue;
    }

    void Update()
    {
        if (drfbManager.fuelId != fuelSlot.itemId || drfbManager.fuelQuantity != fuelSlot.quantity)
        {
            if (fuelSlot.isChangedByUI)
            {
                drfbManager.SetFuelSlotServerRpc(fuelSlot.itemId, fuelSlot.quantity);
                fuelSlot.isChangedByUI = false;
            }
            else
            {
                fuelSlot.itemId = drfbManager.fuelId;
                fuelSlot.quantity = drfbManager.fuelQuantity;
            }
        }
        if (drfbManager.firstResourceId != resourceSlot1.itemId || drfbManager.firstResQuantity != resourceSlot1.quantity)
        {
            if (resourceSlot1.isChangedByUI)
            {
                drfbManager.SetFirstResourceSlotServerRpc(resourceSlot1.itemId, resourceSlot1.quantity);
                resourceSlot1.isChangedByUI = false;
            }
            else
            {
                resourceSlot1.itemId = drfbManager.firstResourceId;
                resourceSlot1.quantity = drfbManager.firstResQuantity;
            }
        }
        if (drfbManager.secondResourceId != resourceSlot2.itemId || drfbManager.secondResQuantity != resourceSlot2.quantity)
        {
            if (resourceSlot2.isChangedByUI)
            {
                drfbManager.SetSecondResourceSlotServerRpc(resourceSlot2.itemId, resourceSlot2.quantity);
                resourceSlot2.isChangedByUI = false;
            }
            else
            {
                resourceSlot2.itemId = drfbManager.secondResourceId;
                resourceSlot2.quantity = drfbManager.secondResQuantity;
            }
        }
        if (drfbManager.productId != productSlot.itemId || drfbManager.prodQuantity != productSlot.quantity)
        {
            if (productSlot.isChangedByUI)
            {
                drfbManager.SetProductSlotServerRpc(productSlot.itemId, productSlot.quantity);
                productSlot.isChangedByUI = false;
            }
            else
            {
                productSlot.itemId = drfbManager.productId;
                productSlot.quantity = drfbManager.prodQuantity;
                if (productSlot.transform.childCount == 0)
                {
                    GameObject item = Instantiate(possibleProducts.Find(x => x.GetComponent<UIItem>().itemId == drfbManager.productId), productSlot.transform);
                    Instantiate(itemAmount, item.transform, false);
                }
               
            }
        }
        fuelSlider.currentValue = drfbManager.fuelSliderValue;
        arrowSlider.currentValue = drfbManager.arrowSliderValue;
        if (fuelSlot.quantity == 0 && fuelSlot.transform.childCount > 0) Destroy(fuelSlot.GetComponentInChildren<UIItem>().gameObject);
        if (resourceSlot1.quantity == 0 && resourceSlot1.transform.childCount > 0) Destroy(resourceSlot1.GetComponentInChildren<UIItem>().gameObject);
        if (resourceSlot2.quantity == 0 && resourceSlot2.transform.childCount > 0) Destroy(resourceSlot2.GetComponentInChildren<UIItem>().gameObject);
        if (productSlot.quantity == 0 && productSlot.transform.childCount > 0) Destroy(productSlot.GetComponentInChildren<UIItem>().gameObject);
    }
}

