using System.Collections.Generic;
using UnityEngine;

public class PriceScreen : MonoBehaviour
{
    [SerializeField] private PriceItem priceItemPrefab;
    [SerializeField] private Transform container;

    private List<ItemData> currentItems = new List<ItemData>();

    private List<PriceItem> itemInfos = new();

    void Start()
    {
        if(ItemSystem.instance.TryGetGroupByID(ItemGroupAPI.Disk, out var group))
        {
            currentItems = group.items;
        }

        Init();
    }

    void OnDestroy()
    {
        itemInfos.Clear();
    }

    private void Init()
    {
        if(currentItems == null || currentItems.Count <= 0)
            return;

        itemInfos.Clear();

        foreach(ItemData item in currentItems)
        {
            PriceItem priceItem = Instantiate(priceItemPrefab, container);
            priceItem.SetData(item);

            itemInfos.Add(priceItem);
        }
    }
}
