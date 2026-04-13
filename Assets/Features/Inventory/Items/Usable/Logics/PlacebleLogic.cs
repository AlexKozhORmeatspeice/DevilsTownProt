using UnityEngine;

public class PlacebleLogic : IUsableLogic
{
    private ItemData item;
    private Usable usable = null;
    private Vector3 usePos;

    private Unit owner;

    public void Init(ItemData data)
    {
        item = data;
    }

    public void SetOwnerUnit(Unit unit)
    {
        owner = unit;
    }

    public void SetUsable(Usable _usable)
    {
        usable = _usable;
    }

    public void SetPos(Vector3 pos)
    {
        usePos = pos;
    }

    public void Use()
    {
        if(usable != null || item.placeblePrefab == null) return;

        //Спауним объект
        GameObject gameObject = GameObject.Instantiate(item.placeblePrefab, usePos, Quaternion.identity);

        //Удаляем предмет из инвентаря
        UnitInventory inv = owner.GetComponent<UnitInventory>();
        if(inv == null) return;

        inv.DeleteItem(inv.Items.IndexOf(item));

        owner.GetComponent<UseComponent>().SetItemToUse(null);
    }

}
