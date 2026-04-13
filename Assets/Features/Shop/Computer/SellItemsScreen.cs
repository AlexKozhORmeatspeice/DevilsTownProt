using System;
using System.Collections.Generic;
using UnityEngine;

public class SellItemsScreen : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private ItemsGroupScreen itemsGroupPrefab;

    private List<ItemGroupsData> items;

    private List<ItemsGroupScreen> groups = new List<ItemsGroupScreen>();

    void Start()
    {
        items = ItemSystem.instance.Groups;
        
        UpdateList();
    }

    public void UpdateList()
    {
        ClearData();
        SpawnDisks();
    }

    private void ClearData()
    {
        foreach(var group in groups)
        {
            Destroy(group.gameObject);
        }
    }

    private void SpawnDisks()
    {
        foreach (var item in items)
        {
            ItemsGroupScreen group = Instantiate(itemsGroupPrefab, container);
            
            group.SetName(item.name);
            group.SetItems(item.items);

            group.gameObject.SetActive(true);

            groups.Add(group);
        }
    }
}
