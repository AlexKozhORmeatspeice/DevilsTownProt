using UnityEngine;

public interface IUsable
{
    public void ProcessUse(ItemData itemData);
}

public class Usable : MonoBehaviour, IUsable
{

    public void ProcessUse(ItemData itemData)
    {
        if(itemData == null)
            return;
    }
}
