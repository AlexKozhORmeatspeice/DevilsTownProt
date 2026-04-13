using UnityEngine;

public class EndSystem : MonoBehaviour
{

    void Update()
    {
        CheckEnd();
    }

    private void CheckEnd()
    {
        if(CurrencyManager.Instance.CurrentMoney >= 10000)
        {
            EndScreen.instance.ShowWin();
        }

        if(BillSystem.instance.LastBills.Count > 4)
        {
            EndScreen.instance.ShowLose();
        }

        if(DevilSystem.Instance.CurrentValue <= -100.0f)
        {
            EndScreen.instance.ShowLose();
        }

        if(InvestSystem.instance.CurrentValue >= 100)
        {
            EndScreen.instance.ShowLose();
        }
    }
}
