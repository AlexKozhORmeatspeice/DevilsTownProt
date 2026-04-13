using UnityEngine;

public class UnitSystem : MonoBehaviour
{
    private static UnitSystem instance;
    public static UnitSystem Instance => instance;

    [SerializeField] private Unit player;
    public Unit Player => player;

    void Awake()
    {
        instance = this;
    }
}
