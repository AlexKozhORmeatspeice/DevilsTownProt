using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

public class PriceSystem : MonoBehaviour
{
    public static PriceSystem instance;

    [Header("Настройки цен и спроса")]
    [SerializeField] private float maxPrice = 1000f;
    [SerializeField] private float minPrice = 0f;
    [SerializeField] private int demandChangeRange = 20; // N: спрос меняется на [-N; +N] каждый день

    [SerializeField] private float[] demandChangesByDevil;
    
    [Header("Базовая формула цены")]
    [SerializeField] private float basePriceMultiplier = 10f; // Цена = Спрос * множитель (но не выходит за границы)
    
    private List<ItemData> itemsList = new List<ItemData>();
    private Dictionary<ItemData, float> marketPriceDictionary = new Dictionary<ItemData, float>();
    private Dictionary<ItemData, float> marketDemandDictionary = new Dictionary<ItemData, float>();
    private Dictionary<ItemData, float> playerPriceDictionary = new Dictionary<ItemData, float>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        TimeSystem.instance.onNewDay += UpdateData;

        if(ItemSystem.instance.TryGetGroupByID(ItemGroupAPI.Disk, out var group))
        {
            InitializeItems(group.items);
        }
    }

    void OnDestroy()
    {
        if (TimeSystem.instance != null)
            TimeSystem.instance.onNewDay -= UpdateData;
    }

    public void InitializeItems(List<ItemData> items)
    {
        itemsList = items;
        
        List<ItemData> shuffledItems = itemsList.OrderBy(x => Random.value).ToList();

        int i = 1;
        float step = 100.0f / shuffledItems.Count;

        foreach (var item in shuffledItems)
        {
            // Начальный спрос: случайное значение от 0 до 100
            float initialDemand = Random.Range((i - 1) * step, i * step);
            marketDemandDictionary[item] = initialDemand;
            
            // Начальная цена: рассчитываем от спроса
            float initialPrice = CalculatePriceFromDemand(initialDemand);
            marketPriceDictionary[item] = initialPrice;
            
            // Начальная цена игрока (можно сделать равной рыночной или чуть ниже)
            playerPriceDictionary[item] = initialPrice;

            i++;
        }
        
        Debug.Log($"PriceSystem: Инициализировано {itemsList.Count} предметов");
    }

    public float GetMarketPrice(ItemData item)
    {
        if (marketPriceDictionary.ContainsKey(item))
            return marketPriceDictionary[item];
        
        return item.basePrice;
    }

    public float GetMarketDemand(ItemData item)
    {
        if (marketDemandDictionary.ContainsKey(item))
            return marketDemandDictionary[item];
        
        Debug.LogWarning($"Нет данных о спросе для {item.name}, возвращаю 0");
        return 0f;
    }

    public float GetPlayerPrice(ItemData item)
    {
        if (playerPriceDictionary.ContainsKey(item))
            return playerPriceDictionary[item];
        
        Debug.LogWarning($"Нет данных о цене игрока для {item.name}, возвращаю 0");
        return 0f;
    }
    
    public void SetPlayerPrice(ItemData item, float price)
    {
        if (playerPriceDictionary.ContainsKey(item))
        {
            playerPriceDictionary[item] = price < 0.0f ? 0.0f : price;
        }
    }

    private void UpdateData()
    {
        Debug.Log("PriceSystem: Обновление дневных данных");
        
        foreach (var item in itemsList)
        {
            // 1. Меняем спрос на случайное значение в диапазоне [-N; +N]
            float currentDemand = marketDemandDictionary[item];
            float demandChange = Random.Range(-demandChangeRange, demandChangeRange + 1);
            float newDemand = Mathf.Clamp(currentDemand + demandChange, 0f, 100f);
            marketDemandDictionary[item] = newDemand;
            
            // 2. Пересчитываем цену от нового спроса
            float newPrice = CalculatePriceFromDemand(newDemand);
            marketPriceDictionary[item] = newPrice;
        }
    }
    
    private float CalculatePriceFromDemand(float demand)
    {
        float price = (demand / 100f) * maxPrice;
        price = (float)Math.Round(price, 2);
        
        return Mathf.Clamp(price, minPrice, maxPrice);
    }


    public float CalculateDemandForUnit(Unit unit, ItemData itemData)
    {
        float resDemand = GetMarketDemand(itemData);

        if(unit.isFavouriteDisk(itemData))
        {
            resDemand += 20.0f;
        }

        float marketPrice = GetMarketPrice(itemData);
        float playerPrice = GetPlayerPrice(itemData);

        float delta = playerPrice - marketPrice;

        resDemand += -delta * 5.0f;

        if(DevilSystem.Instance.CurrentValue <= -75.0f)
        {
            resDemand += demandChangesByDevil[0];
        }
        else if(DevilSystem.Instance.CurrentValue <= -50.0f)
        {
            resDemand += demandChangesByDevil[1];
        }
        else if(DevilSystem.Instance.CurrentValue <= -25.0f)
        {
            resDemand += demandChangesByDevil[2];
        }
        else if(DevilSystem.Instance.CurrentValue < 0.0f)
        {
            resDemand += demandChangesByDevil[3];
        }
        else if(DevilSystem.Instance.CurrentValue >= 0.0f)
        {
            resDemand += demandChangesByDevil[4];
        }
        else if(DevilSystem.Instance.CurrentValue >= 25.0f)
        {
            resDemand += demandChangesByDevil[5];
        }
        else if(DevilSystem.Instance.CurrentValue >= 50.0f)
        {
            resDemand += demandChangesByDevil[6];
        }
        else if(DevilSystem.Instance.CurrentValue >= 75.0f)
        {
            resDemand += demandChangesByDevil[7];
        }
        else if(DevilSystem.Instance.CurrentValue >= 100.0f)
        {
            resDemand += demandChangesByDevil[8];
        }

        resDemand = Mathf.Clamp(resDemand, 0.0f, 100.0f);

        return resDemand;
    }
}