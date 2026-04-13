using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

using Random = UnityEngine.Random;

public class Routine
{
    public Transform workPoint;
    public Transform homePoint;
}

public class AISystem : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] private Transform homeContainer;
    [SerializeField] private Transform workContainer;
    [SerializeField] private int unitLimit = 1;
    [Range(0.0f, 1.0f)] [SerializeField] private float chanceToGoShop = 0.2f;
    [Range(1.0f, 4.0f)] [SerializeField] private float checksCountModifier = 4.0f;

    private List<Transform> homePoints = new();
    private List<Transform> workPoints = new();

    [SerializeField] private Transform shopPoint;

    [SerializeField] private AIContoller aiPrefab;

    private Dictionary<AIContoller, Routine> aiContollers = new();
    private List<AIContoller> contollers = new();

    private bool isCheckingShop = false;
    private int checkShopUnitId = 0;
    private float secondBetweenChecks = 0.5f;
    private float lastTimeCheck = 0.0f;

    void Awake()
    {
        for (int i = 0; i < homeContainer.childCount; i++)
        {
            homePoints.Add(homeContainer.GetChild(i));
        }

        for (int i = 0; i < workContainer.childCount; i++)
        {
            workPoints.Add(workContainer.GetChild(i));
        }

        InitUnits();
    }

    void Start()
    {
        secondBetweenChecks = TimeSystem.instance.WorkDuration * 60.0f / ((float)contollers.Count) / checksCountModifier;
        TimeSystem.instance.OnPhaseChanged += HandleDayPhase;

        TimeSystem.instance.StartTimer();
    }

    void OnDestroy()
    {
        TimeSystem.instance.OnPhaseChanged -= HandleDayPhase;
    }

    void Update()
    {
        int deadCount = contollers.Count(x => x == null);
        CreateNewUnits(deadCount);
        contollers.RemoveAll(x => x == null);

        UpdateGoToShop();
    }

    private void UpdateGoToShop()
    {
        if (!isCheckingShop) return;

        if (Time.time - lastTimeCheck >= secondBetweenChecks)
        {
            lastTimeCheck = Time.time;

            if (contollers.Count == 0) return;

            if (checkShopUnitId >= contollers.Count)
            {
                checkShopUnitId = 0;
            }

            AIContoller ai = contollers[checkShopUnitId];
            checkShopUnitId++;

            if(ai == null)
            {
                return;
            }

            if (Random.value <= chanceToGoShop)
            {
                SetGoToShop(ai);
            }
        }
    }

    private void InitUnits()
    {
        List<Transform> availableHomePoints = new List<Transform>(homePoints);
        List<Transform> availableWorkPoints = new List<Transform>(workPoints);
        
        int unitCount = Mathf.Min(availableHomePoints.Count, availableWorkPoints.Count);
        
        for (int i = 0; i < unitCount; i++)
        {
            if(i > unitLimit) break;

            int randomHomeIndex = Random.Range(0, availableHomePoints.Count);
            Transform selectedHome = availableHomePoints[randomHomeIndex];
            availableHomePoints.RemoveAt(randomHomeIndex);
            
            int randomWorkIndex = Random.Range(0, availableWorkPoints.Count);
            Transform selectedWork = availableWorkPoints[randomWorkIndex];
            availableWorkPoints.RemoveAt(randomWorkIndex);
            
            AIContoller newAI = Instantiate(aiPrefab, selectedHome.position, Quaternion.identity);
            
            Routine routine = new Routine
            {
                homePoint = selectedHome,
                workPoint = selectedWork
            };
            
            aiContollers.Add(newAI, routine);
            contollers.Add(newAI);
        }
    }

    private void CreateNewUnits(int count)
    {
        List<Transform> availableHomePoints = new List<Transform>(homePoints);
        List<Transform> availableWorkPoints = new List<Transform>(workPoints);
        
        int unitCount = Mathf.Min(count, availableHomePoints.Count, availableWorkPoints.Count);
        
        for (int i = 0; i < unitCount; i++)
        {
            if(i > unitLimit) break;

            int randomHomeIndex = Random.Range(0, availableHomePoints.Count);
            Transform selectedHome = availableHomePoints[randomHomeIndex];
            availableHomePoints.RemoveAt(randomHomeIndex);
            
            int randomWorkIndex = Random.Range(0, availableWorkPoints.Count);
            Transform selectedWork = availableWorkPoints[randomWorkIndex];
            availableWorkPoints.RemoveAt(randomWorkIndex);
            
            AIContoller newAI = Instantiate(aiPrefab, selectedHome.position, Quaternion.identity);
            
            Routine routine = new Routine
            {
                homePoint = selectedHome,
                workPoint = selectedWork
            };
            
            aiContollers.Add(newAI, routine);
            contollers.Add(newAI);
        }
    }

    private void HandleDayPhase(TimePhase phase)
    {
        switch (phase)
        {
            case TimePhase.StartOfDay:
                SetGoToWork();
                break;
            case TimePhase.WorkDay:
                SetGoToShopTime();
                break;
            case TimePhase.Night:
                SetGoHome();
                break;
        }   
    }

    private void SetGoToWork()
    {
        isCheckingShop = false;
        
        foreach (var ai in aiContollers.Keys)
        {
            if(ai == null || aiContollers[ai] == null)
            {
                continue;
            }

            ai.SetTarget(aiContollers[ai].workPoint);
        }
    }

    private void SetGoToShopTime()
    {
        lastTimeCheck = Time.time;
        isCheckingShop = true;
    }

    private void SetGoToShop(AIContoller ai)
    {
        ai.GoToShop(aiContollers[ai].homePoint);
    }

    private void SetGoHome()
    {
        isCheckingShop = false;

        foreach (var ai in aiContollers.Keys)
        {
            ai.SetTarget(aiContollers[ai].homePoint);
        }
    }
}