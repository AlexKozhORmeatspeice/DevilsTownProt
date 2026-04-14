using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DevilSystem : MonoBehaviour
{
    public static DevilSystem Instance { get; private set; }

    [SerializeField] private UnitInventory altar;
    [SerializeField] private float minChange = 20.0f;
    [SerializeField] private float maxChange = 40.0f;

    [SerializeField] private float minValue = -100.0f;
    [SerializeField] private float maxValue = 100.0f;
    [SerializeField] private float plusRepForHead = 100.0f;
    [SerializeField] private float useDevilPrice = 10.0f;
    [SerializeField] private int maxDiscAmount = 3;

    private float currentValue = 0.0f;
    public float CurrentValue => currentValue;
    public float NormValue => (currentValue - minValue) / (maxValue - minValue);

    private List<ItemData> devilDiscounts = new();
    public List<ItemData> DevilDiscounts => devilDiscounts;

    private void Awake()
    {
        currentValue = 0.0f;
        Instance = this;
    }

    void Update()
    {
        foreach (var item in altar.Items)
        {
            if(item == null) continue;

            if(item.id.Contains("Head"))
            {
                currentValue += plusRepForHead;
            }
        }

        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);

        altar.ClearInventory();
    }

    void Start()
    {
        TimeSystem.instance.onNewDay += OnNewDay;
    }

    void OnDestroy()
    {
        TimeSystem.instance.onNewDay -= OnNewDay;
    }

    private void OnNewDay()
    {
        currentValue -= Random.Range(minChange, maxChange);
        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);

        devilDiscounts.Clear();
    }

    public void AddDevilDiscount(ItemData itemData)
    {
        if(currentValue - useDevilPrice <= -100)
        {
            return;
        }

        int count = 0;
        foreach(var item in devilDiscounts)
        {
            if(item == itemData)
            {
                count++;
            }
        }

        if(count >= maxDiscAmount)
        {
            return;
        }

        currentValue -= useDevilPrice;

        devilDiscounts.Add(itemData);
    }
}
