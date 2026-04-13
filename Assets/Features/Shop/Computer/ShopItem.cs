using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private TMP_Text descTxt;
    [SerializeField] private TMP_Text buttonDesc;

    private ItemData item;

    public void Update()
    {
        if(item != null)
        {
            buttonDesc.text = $"Купить ({BuySystem.instance.GetBuyPrice(item)})";
        }
    }

    public void SetData(ItemData itemData)
    {
        item = itemData;

        img.sprite = itemData.sprite;
        descTxt.text = itemData.desc;
    }

    public void Buy()
    {
        if(item == null) return;

        BuySystem.instance.TryBuyItem(item);
    }

    public void ChangeDevil()
    {
        DevilSystem.Instance.AddDevilDiscount(item);
    }
}
