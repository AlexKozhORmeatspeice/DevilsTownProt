using UnityEngine;

public class InvestComponent : MonoBehaviour
{
    [SerializeField] private float investInterval = 0.5f;
    [SerializeField] private float viewRadius = 10f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private LayerMask obstacleMask; // Добавьте слой для стен
    
    private float timer = 0f;

    void Awake()
    {
        timer = 0.0f;
    }

    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, viewRadius);
        
        timer += Time.deltaTime;
                    
        foreach (Collider2D col in colliders)
        {
            DeadBody deadBody = col.GetComponent<DeadBody>();
            if (deadBody != null)
            {
                Vector3 directionToTarget = (col.transform.position - transform.position).normalized;
                float angleToTarget = Vector2.Angle(transform.up, -directionToTarget);

                Debug.Log(angleToTarget);
                
                if (angleToTarget < viewAngle / 2f)
                {
                    // Проверка на препятствие между объектом и целью
                    float distanceToTarget = Vector2.Distance(transform.position, col.transform.position);
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask);
                    
                    if (hit.collider == null) // Если луч не уперся в стену
                    {
                        Debug.Log("see body - no obstacles");
                        
                        if (timer >= investInterval)
                        {
                            InvestSystem.instance.AddObserv();
                            timer = 0f;
                        }
                        return;
                    }
                    else
                    {
                        Debug.Log("body is behind a wall: " + hit.collider.name);
                    }
                }
            }
        }
    }
}