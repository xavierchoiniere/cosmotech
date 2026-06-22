using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    public float currentValue;
    public float maxValue;
    public TextMeshProUGUI fuelAmount;
    public Slider slider;
    public Gradient colorGradient;
    public Image fill;

    void Start()
    {
        currentValue = 0;
        slider.maxValue = maxValue;
    }

    void Update()
    {
        if (fuelAmount != null) fuelAmount.text = Mathf.RoundToInt(currentValue).ToString();
        if (slider.value < currentValue) slider.value++;
        if (slider.value > currentValue)
        {
            if (currentValue > 0) slider.value--;
            if (currentValue == 0) slider.value = 0;
        }
        if (currentValue > maxValue) currentValue = maxValue;
        fill.color = colorGradient.Evaluate(currentValue / maxValue);
    }
}
