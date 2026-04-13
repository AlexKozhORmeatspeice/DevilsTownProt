using UnityEngine;
using UnityEngine.EventSystems;

public class InvSlot : MonoBehaviour, IDropHandler
{
    private InvItem currentItem;
    private InventoryScreen screen;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(InventoryScreen inventoryScreen)
    {
        screen = inventoryScreen;
    }

    public void SetItem(InvItem item)
    {
        currentItem = item;
        if (item != null)
        {
            item.transform.SetParent(transform);
            item.transform.localPosition = Vector2.zero;
        }

        screen.SetItemDataToSlot(this, item.GetData());
    }

    public InvItem GetItem()
    {
        return currentItem;
    }

    public bool IsEmpty()
    {
        return currentItem == null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj == null) return;

        InvItem draggedItem = droppedObj.GetComponent<InvItem>();
        if (draggedItem == null) return;

        InvSlot fromSlot = draggedItem.GetCurrentSlot();
        if (fromSlot == this) return;

        if (IsEmpty())
        {
            MoveItemToEmptySlot(draggedItem, fromSlot);
        }
        else
        {
            SwapItems(draggedItem, fromSlot);
        }

        //screen.OnItemMoved(draggedItem, fromSlot, this);
    }

    private void MoveItemToEmptySlot(InvItem draggedItem, InvSlot fromSlot)
    {
        fromSlot.ClearSlot();
        ClearSlot();

        SetItem(draggedItem);
        draggedItem.SetSlot(this);
    }

    private void SwapItems(InvItem draggedItem, InvSlot fromSlot)
    {
        InvItem myItem = currentItem;
        ItemData draggedData = draggedItem.GetData();
        ItemData myData = myItem.GetData();

        fromSlot.ClearSlot();
        ClearSlot();

        fromSlot.SetItem(myItem);
        myItem.SetSlot(fromSlot);

        SetItem(draggedItem);
        draggedItem.SetSlot(this);
    }

    public void ClearSlot()
    {
        currentItem = null;
        screen.SetItemDataToSlot(this, null);
    }
}