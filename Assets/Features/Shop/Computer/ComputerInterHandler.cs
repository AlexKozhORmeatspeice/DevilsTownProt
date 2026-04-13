using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class ComputerInterHandler : MonoBehaviour
{
    private Interactable interactable;

    void Start()
    {
        interactable = GetComponent<Interactable>();

        interactable.onInteract += OnInteract;
    }

    void OnDestroy()
    {
        interactable.onInteract -= OnInteract;
    }

    void OnInteract()
    {
        ComputerScreen.instance.Show();
    }
}
