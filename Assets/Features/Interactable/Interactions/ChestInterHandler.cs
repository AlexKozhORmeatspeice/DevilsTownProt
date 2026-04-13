using UnityEngine;

[RequireComponent(typeof(Interactable), typeof(UnitInventory))]
public class ChestInterHandler : MonoBehaviour
{
    private Interactable interactable;
    private UnitInventory unitInventory;

    void Awake()
    {
        interactable = GetComponent<Interactable>();
        unitInventory = GetComponent<UnitInventory>();

        interactable.onInteract += OnInteract;
    }

    void OnDestroy()
    {
        interactable.onInteract -= OnInteract;
    }

    void OnInteract()
    {
        OpenInv();
    }

    private void OpenInv()
    {
        ChestScreen.instance.Show(unitInventory);
    }
}
