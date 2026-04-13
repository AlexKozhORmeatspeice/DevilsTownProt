using UnityEditor;
using UnityEngine;

public class InvestSystem : MonoBehaviour
{
    public static InvestSystem instance;

    [SerializeField] private float changeForDay = -10.0f;
    [SerializeField] private float investForKill = 10.0f;
    [SerializeField] private float investForObserv = 1.0f;

    [SerializeField] private float minValue = 0;
    [SerializeField] private float maxValue = 100.0f;

    private float currentValue = 0.0f;
    public float CurrentValue => currentValue;
    public float NormValue => (currentValue - minValue) / (maxValue - minValue);

    void Awake()
    {
        instance = this;
        currentValue = 0.0f;
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
        ChangeValue(changeForDay);
    }

    public void ChangeValue(float value)
    {
        currentValue += value;
        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
    }

    public void AddKillUnit()
    {
        ChangeValue(investForKill);
    }

    public void AddObserv()
    {
        ChangeValue(investForObserv);
    }
}
