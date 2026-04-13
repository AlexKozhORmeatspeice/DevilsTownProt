using UnityEngine;

public interface IUsableLogic
{
    void Init(ItemData data);

    void SetOwnerUnit(Unit unit);
    void SetUsable(Usable usable);
    void SetPos(Vector3 pos);
    
    void Use();
}
