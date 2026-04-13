using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CashRegisterSystem : MonoBehaviour
{
    public static CashRegisterSystem instance;

    [SerializeField] private Transform cashRegisterTransform;

    [SerializeField] private TMP_Text infoText;
    [SerializeField] private TMP_Text giveText;
    

    public Transform CashRegisterTransform => cashRegisterTransform;

    private bool isRegister = false;

    private List<ItemData> currentItems = new();

    private float itemsPrice = 0;
    private float currentPayment = 0;
    private float playerGiveMoney = 0;

    public event Action onCompletePayment;


    private void Awake()
    {
        instance = this;
        Hide();
    }

    private void Update()
    {
        giveText.text = "Сдача: " + playerGiveMoney + " $";

        if (currentItems == null || currentItems.Count <= 0 || !isRegister)
        {
            infoText.text = "";

            return;
        } 
        
        string infoRes = "Список предметов:\n";

        foreach (ItemData item in currentItems)
        {
            if(item == null) continue;

            infoRes += "- " + item.showName + ": " + PriceSystem.instance.GetPlayerPrice(item) + " $\n";
        }

        infoRes += "\n";

        infoRes += "Итого: " + itemsPrice + " $\n";
        infoRes += "Дали: " + currentPayment + " $\n";
        infoRes += "Сдача: " + (currentPayment - itemsPrice) + " $\n";

        infoText.text = infoRes;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void CompletePayment()
    {
        if(currentItems == null || currentItems.Count <= 0) return;

        float giveMoney = currentPayment - itemsPrice;
        float plusMoney = currentPayment - playerGiveMoney;

        if(playerGiveMoney <= giveMoney)
        {
            Debug.Log("Not enough give money " + playerGiveMoney + " / " + giveMoney);
            return;
        }

        if(!CurrencyManager.Instance.TryChangeMoney(plusMoney))
        {
            Debug.Log("Not enough money " + CurrencyManager.Instance.CurrentMoney + " / " + plusMoney);
            return;
        }

        currentItems.Clear();
        currentItems = null;

        onCompletePayment?.Invoke();

        isRegister = false;
        itemsPrice = 0;
        currentPayment = 0;
        playerGiveMoney = 0;
    }

    public void ClearGiveMoney()
    {
        playerGiveMoney = 0;
    }

    public void GiveMoney(float money)
    {
        playerGiveMoney += money;
    }

    public bool TryRegisterForPayment(UnitInventory inv)
    {
        if(isRegister || inv.Items == null || inv.Items.Count <= 0 || inv.Items.All(x => x == null))
        {
            return false;
        } 

        isRegister = true;

        playerGiveMoney = 0;

        currentItems = new();
        List<ItemData> items = inv.ClearInventory().Where(x => x != null).ToList();
        currentItems.AddRange(items);

        List<float> paymanetVars = new List<float>();

        foreach(ItemData item in currentItems)
        {
            itemsPrice += PriceSystem.instance.GetPlayerPrice(item);
        }

        float plusVal = (float)Math.Round(UnityEngine.Random.Range(0.0f, 100.0f), 2);
        currentPayment = itemsPrice + plusVal;

        return true;
    }
}
