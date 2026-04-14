using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitInventory : MonoBehaviour
{
    [SerializeField] private List<ItemData> baseItems;
    [SerializeField] private int slotsCount = 4;

    public Action<int> onDeleteItem;

    public int SlotsCount => slotsCount;

    private List<ItemData> items = new();
    public List<ItemData> Items => items;

    void Awake()
    {
        if(baseItems != null)
        {
            items.AddRange(baseItems);
        }
    }

    public void AddItem(ItemData item)
    {
        if(items.Count(x => x != null) >= slotsCount)
        {
            Debug.Log("Not enough space in inventory");
            return;
        }
        
        for(int i = 0; i < items.Count; i++)
        {
            if(items[i] == null)
            {
                items[i] = item;
                return;
            }
        }

        items.Add(item);   
    }
    public void DeleteItem(int ind)
    {
        if(ind >= items.Count) return;
        
        items[ind] = null;
        onDeleteItem?.Invoke(ind);
    }

    public List<ItemData> ClearInventory()
    {
        List<ItemData> inventory = new List<ItemData>();

        for(int i = 0; i < items.Count; i++)
        {
            inventory.Add(items[i]);

            items[i] = null;
            onDeleteItem?.Invoke(i);
        }

        return inventory;
    }

    public void Update()
    {
        // Debug.Log("===================");
        // foreach(ItemData item in items)
        // {
        //     Debug.Log("item " + item);
        // }
    }


}
