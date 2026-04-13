using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightShower : MonoBehaviour
{
    [SerializeField] private float minVal = 0.7f;
    [SerializeField] private float maxVal = 1.0f;

    private Light2D light2D;

    void Awake()
    {
        light2D = GetComponent<Light2D>();
    }

    void Update()
    {
        light2D.intensity = Mathf.Lerp(maxVal, minVal, TimeSystem.instance.NormTime);
    }
}
