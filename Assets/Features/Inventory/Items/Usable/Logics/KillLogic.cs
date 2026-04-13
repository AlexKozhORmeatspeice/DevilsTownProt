using UnityEngine;

public class KillLogic : IUsableLogic
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
        if(usable == null) return;

        Unit unit = usable.gameObject.GetComponent<Unit>();

        if(unit == null) return;

        unit.Kill();
    }
}
