using System;
using Unity.VisualScripting;
using UnityEngine;

public class BillScreen : MonoBehaviour
{
    [SerializeField] private BillUI billPrefab;
    [SerializeField] private Transform billParent;


    public void Start()
    {
        BillSystem.instance.onNewBill += CreateBill;
    }

    void OnDestroy()
    {
        BillSystem.instance.onNewBill -= CreateBill;
    }

    private void CreateBill(Bill bill)
    {
        BillUI billUI = Instantiate(billPrefab, billParent);
        billUI.SetBill(bill);
    }
}
