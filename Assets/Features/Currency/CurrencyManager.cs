using System.ComponentModel;
using UnityEditor.Rendering;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set;}

    [SerializeField] private float baseMoney = 100;

    private float currentMoney = 0;
    public float CurrentMoney => currentMoney;

    public void Awake()
    {
        Instance = this;
        currentMoney = baseMoney;
    }

    public bool IsGotMoney(float val)
    {
        return currentMoney >= val;
    }

    public bool TryChangeMoney(float val)
    {
        if(currentMoney + val < 0)
        {
            return false;
        }

        currentMoney += val;
        return true;
    }
}
