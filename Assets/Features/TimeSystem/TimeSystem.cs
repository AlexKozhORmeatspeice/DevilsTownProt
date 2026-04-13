using UnityEngine;
using System;

public enum TimePhase
{
    StartOfDay,
    WorkDay,
    Night
}

public class TimeSystem : MonoBehaviour
{
    public static TimeSystem instance;

    public event Action<TimePhase> OnPhaseChanged;
    public event Action onNewDay;

    [SerializeField] private float startOfDayDuration = 5f;
    [SerializeField] private float workDayDuration = 5f;
    [SerializeField] private float nightDuration = 5f;

    private float dayDurationMinutes = 15f;
    
    public float MaxTime => dayDurationMinutes;

    public float NormTime => currentTime / dayDurationMinutes;

    public float WorkDuration => workDayDuration;
    
    private float currentTime;
    public float CurrentTime => currentTime;


    private TimePhase currentPhase;
    private float phaseTimer;

    void Awake()
    {
        dayDurationMinutes = startOfDayDuration + workDayDuration + nightDuration;
        instance = this;
    }

    public void StartTimer()
    {
        currentTime = 0f;
        SetPhase(TimePhase.StartOfDay);
    }

    void Update()
    {
        currentTime += Time.deltaTime / 60f;
        phaseTimer += Time.deltaTime;

        float currentPhaseDuration = GetPhaseDuration(currentPhase);

        if(currentTime > dayDurationMinutes)
        {
            currentTime = 0.0f;
            SetPhase(TimePhase.StartOfDay);
        }
        
        if (phaseTimer >= currentPhaseDuration)
        {
            NextPhase();
        }
    }

    private void NextPhase()
    {
        switch (currentPhase)
        {
            case TimePhase.StartOfDay:
                SetPhase(TimePhase.WorkDay);
                break;
            case TimePhase.WorkDay:
                SetPhase(TimePhase.Night);
                break;
            case TimePhase.Night:
                SetPhase(TimePhase.StartOfDay);
                break;
        }
    }

    private void SetPhase(TimePhase newPhase)
    {
        currentPhase = newPhase;
        phaseTimer = 0f;
        OnPhaseChanged?.Invoke(currentPhase);

        if(currentPhase == TimePhase.StartOfDay)
        {
            onNewDay?.Invoke();
        }
    }

    private float GetPhaseDuration(TimePhase phase)
    {
        switch (phase)
        {
            case TimePhase.StartOfDay:
                return startOfDayDuration * 60f;
            case TimePhase.WorkDay:
                return workDayDuration * 60f;
            case TimePhase.Night:
                return nightDuration * 60f;
            default:
                return 0f;
        }
    }

    public string GetCurrentStateName()
    {
        switch (currentPhase)
        {
            case TimePhase.StartOfDay:
                return "Утро";
            case TimePhase.WorkDay:
                return "День";
            case TimePhase.Night:
                return "Ночь";
        }

        return "";
    }
}