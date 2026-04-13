using UnityEngine;


public enum UsableType
{
    Placeble,
    OnUnit,
    Kill,
    None
}

[CreateAssetMenu(menuName = "SO/Inv/ItemData")]
public class ItemData : ScriptableObject
{
    public string id;
    public Sprite sprite;

    public string desc;
    public string showName;

    public float basePrice;

    public UsableType usableType;
    public GameObject placeblePrefab;
}
