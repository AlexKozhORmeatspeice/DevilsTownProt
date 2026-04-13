using UnityEngine;
using System.Collections.Generic;

public class InventoryScreen : MonoBehaviour
{
    [Header("Objs")]
    [SerializeField] private Transform container;
    [SerializeField] private UnitInventory unitInventory;
    [SerializeField] private InvSlot slotPrefab;
    [SerializeField] private InvItem itemPrefab;

    private List<InvSlot> slots = new List<InvSlot>();
    private bool isDirty = true;

    private void Start()
    {
        UpdateInv();

        if(unitInventory != null)
            unitInventory.onDeleteItem += OnDeleteItem;
    }

    public void OnDestroy()
    {
        if(unitInventory != null)
            unitInventory.onDeleteItem -= OnDeleteItem;
    }

    public void SetInventory(UnitInventory _unitInventory)
    {
        if(unitInventory != null)
            unitInventory.onDeleteItem -= OnDeleteItem;
        
        unitInventory = _unitInventory;
        unitInventory.onDeleteItem += OnDeleteItem;

        isDirty = true;
    }

    public void UpdateInv()
    {
        if(unitInventory == null || !isDirty) return;

        CreateSlots();
        FillSlots();

        isDirty = false;
    }

    private void OnDeleteItem(int ind)
    {
        isDirty = true;

        UpdateInv();
    }

    private void CreateSlots()
    {
        foreach (var slot in slots)
        {
            Destroy(slot.gameObject);
        }
        slots.Clear();

        for (int i = 0; i < unitInventory.SlotsCount; i++)
        {
            InvSlot slot = Instantiate(slotPrefab, container);
            slot.Initialize(this);
            slots.Add(slot);
        }
    }

    private void FillSlots()
    {
        for (int i = 0; i < unitInventory.Items.Count && i < slots.Count; i++)
        {
            if (unitInventory.Items[i] != null)
            {
                InvItem item = Instantiate(itemPrefab, slots[i].transform);
                item.Initialize(unitInventory.Items[i], slots[i]);
                slots[i].SetItem(item);
            }
        }
    }

    public void OnItemMoved(InvItem item, InvSlot fromSlot, InvSlot toSlot)
    {
        int fromIndex = slots.IndexOf(fromSlot);
        int toIndex = slots.IndexOf(toSlot);

        if (fromIndex != -1 && toIndex != -1)
        {
            ItemData tempData = unitInventory.Items[fromIndex];
            unitInventory.Items[fromIndex] = unitInventory.Items[toIndex];
            unitInventory.Items[toIndex] = tempData;
        }
    }

    public ItemData GetItemDataFromSlot(InvSlot slot)
    {
        int index = slots.IndexOf(slot);

        if (index != -1 && index < unitInventory.Items.Count)
            return unitInventory.Items[index];
        
        return null;
    }

    public void SetItemDataToSlot(InvSlot slot, ItemData data)
    {
        int index = slots.IndexOf(slot);

        if (index != -1)
        {
            while (unitInventory.Items.Count <= index)
                unitInventory.Items.Add(null);
            
            unitInventory.Items[index] = data;
        }
    }
}