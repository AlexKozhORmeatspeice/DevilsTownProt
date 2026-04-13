using UnityEngine;

public class ChestScreen : MonoBehaviour
{
    [SerializeField] private InventoryScreen inventoryScreen;

    public static ChestScreen instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Hide();
    }

    public void Show(UnitInventory inventory)
    {
        inventoryScreen.SetInventory(inventory);
        inventoryScreen.UpdateInv();

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
