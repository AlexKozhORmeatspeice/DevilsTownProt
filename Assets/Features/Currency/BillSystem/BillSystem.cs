using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class Bill
{
    public float value;
}

public class BillSystem : MonoBehaviour
{
    public static BillSystem instance;

    [SerializeField] private float minBill = 10.0f;
    [SerializeField] private float maxBill = 30.0f;

    private List<Bill> lastBills = new();
    public List<Bill> LastBills => lastBills;

    public event Action<Bill> onNewBill;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        TimeSystem.instance.onNewDay += CreateBill;
    }

    void OnDestroy()
    {
        TimeSystem.instance.onNewDay -= CreateBill;
    }

    private void CreateBill()
    {
        float bill = Random.Range(minBill, maxBill);
        Bill objBill = new();
        objBill.value = bill;
        
        lastBills.Add(objBill);

        onNewBill?.Invoke(objBill);
    }

    public void ClearBill(Bill bill)
    {
        lastBills.Remove(bill);
    }
}
