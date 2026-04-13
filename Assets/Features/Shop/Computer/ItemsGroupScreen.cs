using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ItemsGroupScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private Transform container;
    [SerializeField] private ShopItem itemPrefab;

    private List<ShopItem> items = new List<ShopItem>();

    public void SetName(string name)
    {
        nameTxt.text = name;
    }

    public void SetItems(List<ItemData> _items)
    {
        foreach (ItemData item in _items)
        {
            ShopItem shopItem = Instantiate(itemPrefab, container);
            shopItem.SetData(item);

            items.Add(shopItem);
        }
    }
}
