using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemGroupsData
{
    public string name;
    public string id;
    public List<ItemData> items;
}

public class ItemSystem : MonoBehaviour
{
    public static ItemSystem instance;

    [SerializeField] private List<ItemGroupsData> itemGroups;

    private List<ItemData> allItems = new List<ItemData>();

    public List<ItemGroupsData> Groups => itemGroups;
    public List<ItemData> AllItems => allItems;

    void Awake()
    {
        instance = this;
        
        foreach (var itemGroup in itemGroups)
        {
            allItems.AddRange(itemGroup.items);
        }
    }

    public bool TryGetGroupByID(string id, out ItemGroupsData itemGroupsData)
    {
        var group = itemGroups.Find(g => g.id == id);
        itemGroupsData = group;

        return group.items != null && group.items.Count > 0;
    }
}
