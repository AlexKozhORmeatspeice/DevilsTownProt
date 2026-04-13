using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DevilScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Slider slider;

    void Update()
    {
        text.text = DevilSystem.Instance.CurrentValue.ToString();
        slider.value = DevilSystem.Instance.NormValue;
    }
}
