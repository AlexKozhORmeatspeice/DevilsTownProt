using TMPro;
using UnityEngine;

public class CurrencyShower : MonoBehaviour
{
    [SerializeField] private TMP_Text currencyTxt;

    void Update()
    {
        if(currencyTxt == null) return;

        currencyTxt.text = "Текущая валюта: " + CurrencyManager.Instance.CurrentMoney.ToString();
    }
}
