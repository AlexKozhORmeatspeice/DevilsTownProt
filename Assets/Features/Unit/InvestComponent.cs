using UnityEngine;

public class InvestComponent : MonoBehaviour
{
    [SerializeField] private float investInterval = 0.5f;
    [SerializeField] private float viewRadius = 10f;
    [SerializeField] private float viewAngle = 90f;
    
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
            if (col.GetComponent<DeadBody>() != null)
            {
                Vector3 directionToTarget = (col.transform.position - transform.position).normalized;
                float angleToTarget = Vector2.Angle(transform.up, -directionToTarget);
                
                if (angleToTarget < viewAngle / 2f)
                {
                    Debug.Log("see body");

                    if (timer >= investInterval)
                    {
                        InvestSystem.instance.AddObserv();
                        timer = 0f;
                    }
                    return;
                }
            }
        }
    }
}