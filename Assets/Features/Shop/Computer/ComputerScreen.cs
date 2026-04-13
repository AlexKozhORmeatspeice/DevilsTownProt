using UnityEngine;

public class ComputerScreen : MonoBehaviour
{
    public static ComputerScreen instance;
    void Awake()
    {
        instance = this;
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
