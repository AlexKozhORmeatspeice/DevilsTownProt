using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvestScreen : MonoBehaviour
{   
    [SerializeField] private TMP_Text text;
    [SerializeField] private Slider slider;

    void Update()
    {
        text.text = InvestSystem.instance.CurrentValue.ToString();
        slider.value = InvestSystem.instance.NormValue;
    }
}
