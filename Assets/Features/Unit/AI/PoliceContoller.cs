using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class PoliceController : MonoBehaviour
{
    [Header("Waypoints Settings")]
    [SerializeField] private Transform waypointsParent; // Родительский объект, содержащий все точки
    [SerializeField] private int pointsToVisit = 5; // Количество точек для посещения (максимум 5)
    [SerializeField] private bool loop = true; // Зацикливать движение
    [SerializeField] private bool randomizeRouteEachCycle = true; // Создавать новый случайный маршрут после каждого цикла
    
    [Header("Movement Settings")]
    [SerializeField] private float stoppingDistance = 0.5f; // Дистанция до точки для смены цели
    [SerializeField] private float waitTimeAtWaypoint = 1f; // Время ожидания на точке
    
    [Header("NPC Settings")]
    [SerializeField] private bool startAutomatically = true; // Начинать движение автоматически

    private UnitMovement unitMovement;
    
    // Приватные переменные
    private NavMeshAgent agent;
    private List<Transform> allWaypoints = new List<Transform>(); // Все доступные точки
    private List<Transform> currentRoute = new List<Transform>(); // Текущий маршрут из 5 точек
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    
    void Start()
    {
        unitMovement = GetComponent<UnitMovement>();

        // Получаем компонент NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent компонент не найден на объекте " + gameObject.name);
            return;
        }
        
        // Настройка агента для 2D (отключаем вращение по Y)
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        // Собираем все дочерние точки
        CollectWaypoints();
        
        if (allWaypoints.Count == 0)
        {
            Debug.LogWarning("Нет дочерних точек для перемещения у NPC " + gameObject.name);
            return;
        }
        
        // Создаем случайный маршрут
        CreateRandomRoute();
        
        if (startAutomatically)
        {
            MoveToNextWaypoint();
        }
    }
    
    void Update()
    {
        if (agent == null || currentRoute.Count == 0) return;

        if(unitMovement != null)
        {
            Vector3 dir = agent.velocity.normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            unitMovement.SetRotation(angle);
        }
        
        // Если NPC ожидает, обрабатываем таймер
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                MoveToNextWaypoint();
            }
            return;
        }
        
        // Проверяем, достиг ли NPC текущей точки
        if (!agent.pathPending && agent.remainingDistance <= stoppingDistance)
        {
            // Достигли точки
            if (waitTimeAtWaypoint > 0)
            {
                StartWaiting();
            }
            else
            {
                MoveToNextWaypoint();
            }
        }
    }
    
    void CollectWaypoints()
    {
        allWaypoints.Clear();
        
        if (waypointsParent == null)
        {
            Debug.LogError("Waypoints Parent не назначен в инспекторе!");
            return;
        }
        
        // Собираем все дочерние объекты
        foreach (Transform child in waypointsParent)
        {
            allWaypoints.Add(child);
        }
        
        Debug.Log($"Собрано {allWaypoints.Count} точек для маршрутов");
    }
    
    void CreateRandomRoute()
    {
        if (allWaypoints.Count == 0) return;
        
        currentRoute.Clear();
        
        // Определяем сколько точек будем использовать (не больше доступного количества)
        int pointsToUse = Mathf.Min(pointsToVisit, allWaypoints.Count);
        
        if (pointsToUse == 0) return;
        
        // Создаем копию списка для случайной выборки
        List<Transform> availablePoints = new List<Transform>(allWaypoints);
        
        // Выбираем случайные уникальные точки
        for (int i = 0; i < pointsToUse; i++)
        {
            int randomIndex = Random.Range(0, availablePoints.Count);
            currentRoute.Add(availablePoints[randomIndex]);
            availablePoints.RemoveAt(randomIndex);
        }
        
        Debug.Log($"Создан маршрут из {currentRoute.Count} точек");
        
        // Опционально: выводим маршрут в консоль для отладки
        string routeInfo = "Маршрут: ";
        foreach (var point in currentRoute)
        {
            routeInfo += point.name + " -> ";
        }
        Debug.Log(routeInfo);
    }
    
    void MoveToNextWaypoint()
    {
        if (currentRoute.Count == 0) return;
        
        // Проверяем, достигли ли конца маршрута
        if (currentWaypointIndex >= currentRoute.Count)
        {
            if (loop)
            {
                currentWaypointIndex = 0;
                
                if (randomizeRouteEachCycle)
                {
                    // Создаем новый случайный маршрут для следующего цикла
                    CreateRandomRoute();
                }
            }
            else
            {
                // Достигли конца и не зациклено - останавливаемся
                agent.isStopped = true;
                return;
            }
        }
        
        // Проверяем, не пустой ли маршрут после возможного обновления
        if (currentRoute.Count == 0 || currentWaypointIndex >= currentRoute.Count)
        {
            return;
        }
        
        // Устанавливаем цель для движения
        Transform targetPoint = currentRoute[currentWaypointIndex];
        if (targetPoint != null)
        {
            agent.SetDestination(targetPoint.position);
            agent.isStopped = false;
            currentWaypointIndex++;
        }
        else
        {
            Debug.LogWarning($"Точка с индексом {currentWaypointIndex - 1} не существует!");
            MoveToNextWaypoint(); // Пропускаем null точку
        }
    }
    
    void StartWaiting()
    {
        isWaiting = true;
        waitTimer = waitTimeAtWaypoint;
        agent.isStopped = true; // Останавливаем движение во время ожидания
    }
    
    // Публичные методы для управления извне
    
    public void RefreshWaypoints()
    {
        CollectWaypoints();
        CreateRandomRoute();
        currentWaypointIndex = 0;
        if (startAutomatically)
        {
            MoveToNextWaypoint();
        }
    }
    
    public void CreateNewRandomRoute()
    {
        CreateRandomRoute();
        currentWaypointIndex = 0;
        if (!isWaiting && startAutomatically)
        {
            MoveToNextWaypoint();
        }
    }
    
    public void StopMovement()
    {
        agent.isStopped = true;
        isWaiting = false;
    }
    
    public void ResumeMovement()
    {
        agent.isStopped = false;
        if (!isWaiting && (agent.remainingDistance <= stoppingDistance))
        {
            MoveToNextWaypoint();
        }
    }
    
    public void ResetPath()
    {
        currentWaypointIndex = 0;
        MoveToNextWaypoint();
    }
    
    public bool IsMoving()
    {
        return !isWaiting && !agent.isStopped && agent.remainingDistance > stoppingDistance;
    }
    
    public List<Transform> GetCurrentRoute()
    {
        return new List<Transform>(currentRoute);
    }
    
    public Transform GetCurrentTarget()
    {
        if (currentWaypointIndex > 0 && currentWaypointIndex <= currentRoute.Count)
        {
            return currentRoute[currentWaypointIndex - 1];
        }
        return null;
    }
    
    // Визуализация в редакторе
    void OnDrawGizmosSelected()
    {
        if (waypointsParent == null) return;
        
        // Рисуем все доступные точки
        Gizmos.color = Color.gray;
        foreach (Transform child in waypointsParent)
        {
            if (child != null)
            {
                Gizmos.DrawWireSphere(child.position, 0.2f);
            }
        }
        
        // Рисуем текущий маршрут (только в режиме игры)
        if (Application.isPlaying && currentRoute.Count > 0)
        {
            for (int i = 0; i < currentRoute.Count - 1; i++)
            {
                if (currentRoute[i] != null && currentRoute[i + 1] != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(currentRoute[i].position, currentRoute[i + 1].position);
                }
            }
            
            // Рисуем текущую цель
            if (GetCurrentTarget() != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(GetCurrentTarget().position, 0.4f);
            }
        }
        
        // В редакторе рисуем пример соединения всех точек
        if (!Application.isPlaying && allWaypoints.Count > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < allWaypoints.Count - 1; i++)
            {
                if (allWaypoints[i] != null && allWaypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(allWaypoints[i].position, allWaypoints[i + 1].position);
                }
            }
        }
    }
}