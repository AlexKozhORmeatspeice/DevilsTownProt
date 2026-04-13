using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnitInventory), typeof(NavMeshAgent))]
public class AIContoller : MonoBehaviour
{
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float randomMoveRadius = 3f;
    [Range(0.0f, 1.0f)] [SerializeField] private float chanceToGoGetDisk = 0.6f;
    [Range(0.0f, 3.0f)] [SerializeField] private float secondsBetweenCheck = 1.0f;
    [Range(1, 5)] [SerializeField] private int checksNumForDisk = 3;
    
    private UnitInventory unitInventory;
    private NavMeshAgent agent;
    private UnitMovement unitMovement;
    private Unit unit;

    private Transform currentTarget;

    private int currentPointIndex = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private bool isRandomMoving = false;
    private Vector3 randomTarget;
    private float randomMoveTimer = 0f;

    private bool isCheckingShop = false;
    private int numChecks = 0;
    private float lastTimeCheck = 0.0f;

    private bool isPaying = false;

    private ChestInterHandler toCheckChest = new();
    private List<ChestInterHandler> allChests = new();

    private Transform homePoint;

    void Awake()
    {
        unitMovement = GetComponent<UnitMovement>();
        unit = GetComponent<Unit>();
        agent = GetComponent<NavMeshAgent>();
        unitInventory = GetComponent<UnitInventory>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        if(currentTarget == null)
            return;
        
        UpdateRotation();

        if(isCheckingShop)
        {
            CheckShop();
        }
        else if(isPaying)
        {
            CheckPay();
        }
        else
        {
            GoToPoint();
        }
    }

    private void CheckPay()
    {
        if(Vector2.Distance(transform.position, currentTarget.position) > 4.0f)
        {
            return;
        };

        if(CashRegisterSystem.instance.TryRegisterForPayment(unitInventory))
        {
            CashRegisterSystem.instance.onCompletePayment += CompletePay;
        }
    }

    private void CompletePay()
    {
        Debug.Log("Complete pay");
        
        CashRegisterSystem.instance.onCompletePayment -= CompletePay;

        isPaying = false;
        isCheckingShop = false;

        GoHome();
    }

    private void CheckShop()
    {
        if(!TryChooseChest())
        {
            if(!TryGetChests() && !TryChooseChest())
            {
                if(numChecks > 0 && unitInventory.Items.Any(x => x != null && x.id.Contains("Disk")))
                {
                    GoToPay();
                }
                else
                {
                    GoHome();
                }
            }

            return;
        }
        
        if (Vector2.Distance(transform.position, toCheckChest.transform.position) > 4.0f)
        {
            lastTimeCheck = Time.time;
            return;
        }

        if(numChecks >= checksNumForDisk)
        {
            if(unitInventory.Items.Count > 0)
            {
                GoToPay();
                return;
            }

            GoHome();
            return;
        }

        CheckChestForDisk();
    }

    private void CheckChestForDisk()
    {
        if(Time.time - lastTimeCheck >= secondsBetweenCheck)
        {
            lastTimeCheck = Time.time;

            if(!TryChooseChest())
            {
                numChecks++;
                return;
            }

            UnitInventory chestInv = toCheckChest.GetComponent<UnitInventory>();

            if(chestInv.Items.Count >= 0)
            {
                ItemData item = chestInv.Items.FindAll(x => x != null && x.id.Contains("Disk")).OrderBy(x => Random.value).FirstOrDefault();

                if(item != null)
                {
                    float rndVal = Random.value;
                    float demand = PriceSystem.instance.CalculateDemandForUnit(unit, item);

                    if(rndVal <= demand)
                    {
                        int ind = chestInv.Items.IndexOf(item);

                        chestInv.DeleteItem(ind);
                        unitInventory.AddItem(item);
                    }
                }
            }

            numChecks++;
        }
    }

    private void GoToPay()
    {
        isPaying = true;
        isCheckingShop = false;
        isRandomMoving = false;

        currentTarget = CashRegisterSystem.instance.CashRegisterTransform;
        agent.SetDestination(currentTarget.position);

        Debug.Log("Start pay");
    }

    private void GoToPoint()
    {
        // Если сейчас случайное движение
        if (isRandomMoving)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
            }

            // Если достигли случайной точки, ставим новую случайную цель
            if (Vector2.Distance(transform.position, randomTarget) < 0.5f && !isWaiting)
            {
                waitTimer = 0.0f;

                isWaiting = true;
                SetRandomTarget();
            }

            return;
        }

        // Если достигли точки патруля и не ждем и не двигаемся случайно
        if (!agent.pathPending && agent.remainingDistance < 0.5f && !isWaiting && !isRandomMoving)
        {
            StartRandomMovement();
            isWaiting = false;

            waitTimer = 0f;
        }
    }

    private void GoHome()
    {
        isCheckingShop = false;
        isPaying = false;

        SetTarget(homePoint);
    }

    private void StartRandomMovement()
    {
        isRandomMoving = true;
        randomMoveTimer = 0f;

        SetRandomTarget();
    }

    private void SetRandomTarget()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-randomMoveRadius, randomMoveRadius), 
            Random.Range(-randomMoveRadius, randomMoveRadius),
            0.0f
        );

        Vector3 pointPos = currentTarget.position;

        randomTarget = pointPos + randomOffset;

        agent.SetDestination(randomTarget);
    }

    private void UpdateRotation()
    {
        Vector3 dir = agent.velocity.normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        unitMovement.SetRotation(angle);
    }

    public void SetTarget(Transform target)
    {
        if(isCheckingShop || isPaying) return;

        currentTarget = target;
        agent.SetDestination(currentTarget.position);

        isRandomMoving = false;
        isWaiting = false;
    }

    public void GoToShop(Transform afterHomePoint)
    {
        toCheckChest = null;

        isCheckingShop = true;
        isPaying = false;

        homePoint = afterHomePoint;

        if(!TryGetChests())
        {
            GoHome();
            return;
        }


        if(!TryChooseChest())
        {
            GoHome();
            return;
        }

        numChecks = 0;
    }

    private bool TryGetChests()
    {
        List<ChestInterHandler> tempChests = GameObject.FindObjectsByType<ChestInterHandler>().ToList();

        if(tempChests == null || tempChests.Count == 0)
        {
            return false;
        }

        allChests.Clear();

        foreach(var chest in tempChests)
        {
            var inv = chest.gameObject.GetComponent<UnitInventory>();

            if(inv != null && inv.Items != null && inv.Items.Count > 0 && inv.Items.Any(x => x != null && x.id.Contains("Disk")))
            {
                allChests.Add(chest);
            }
        }

        toCheckChest = null;

        return allChests != null && allChests.Count > 0;
    }

    private bool TryChooseChest()
    {
        if(allChests == null || allChests.Count == 0)
        {
            return false;
        }

        if(toCheckChest != null)
        {
            UnitInventory chestInv = toCheckChest.gameObject.GetComponent<UnitInventory>();
            return chestInv.Items != null && chestInv.Items.Count > 0 && chestInv.Items.Any(x => x != null && x.id.Contains("Disk"));
        }

        toCheckChest = allChests[Random.Range(0, allChests.Count)];

        currentTarget = toCheckChest.transform;
        agent.SetDestination(currentTarget.position);

        return toCheckChest != null && toCheckChest.gameObject.GetComponent<UnitInventory>().Items.Any(x => x != null && x.id.Contains("Disk"));
    }
}