using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class CashRegOpener : MonoBehaviour
{
    private Interactable interactable;

    void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    void Start()
    {
        interactable.onInteract += OpenScreen;
    }

    void OnDestroy()
    {
        interactable.onInteract -= OpenScreen;
    }

    public void OpenScreen()
    {
        CashRegisterSystem.instance.Show();
    }
}
