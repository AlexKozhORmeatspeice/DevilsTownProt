using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BillUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private Bill bill;
    
    public void SetBill(Bill _bill)
    {
        bill = _bill;
        text.text = bill.value.ToString();
    }

    public void CLoseBill()
    {
        if(!CurrencyManager.Instance.TryChangeMoney(-bill.value))
        {
            Debug.Log("Not enough money to pay the bill");
            return;
        }

        BillSystem.instance.ClearBill(bill);
        Destroy(gameObject);
    }    
}
