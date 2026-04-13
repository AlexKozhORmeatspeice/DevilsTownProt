using UnityEngine;

public class DeadBody : MonoBehaviour
{
    [SerializeField] private float lifeTime = 10.0f;

    private float spawnTime;

    void Awake()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        if(Time.time - spawnTime > lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
