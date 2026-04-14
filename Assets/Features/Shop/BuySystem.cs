using System.Linq;
using Unity.Collections;
using UnityEngine;

public class BuySystem : MonoBehaviour
{
    public static BuySystem instance;

    [SerializeField] private UnitInventory inventoryToAdd;
    [SerializeField] private float devilDiscount = 0.20f;

    void Awake()
    {
        instance = this;
    }

    public float GetBuyPrice(ItemData item)
    {
        float devilPrice = PriceSystem.instance.GetMarketPrice(item);
        devilPrice *= Mathf.Pow(1.0f-devilDiscount, DevilSystem.Instance.DevilDiscounts.Count(x => x == item));

        return devilPrice;
    }

    public bool TryBuyItem(ItemData item)
    {
        if(inventoryToAdd.SlotsCount < inventoryToAdd.Items.Count)
        {
            Debug.Log("Not enough space in inventory");
            return false;
        }

        if(!CurrencyManager.Instance.TryChangeMoney(-GetBuyPrice(item)))
        {
            Debug.Log("Not enough money");
            return false;
        }
    
        inventoryToAdd.AddItem(item);

        return true;
    }
}
