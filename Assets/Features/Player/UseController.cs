using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UseComponent), typeof(UnitInventory))]
public class UseController : MonoBehaviour
{
    [SerializeField] private InputActionReference useKey;
    [SerializeField] private InputActionReference interactKey;
    [SerializeField] private float interactDist = 10.0f;
    [SerializeField] private List<InputActionReference> inventoryKeys;

    private UseComponent useComponent;
    private UnitInventory unitInventory;


    void Awake()
    {
        useComponent = GetComponent<UseComponent>();
        unitInventory = GetComponent<UnitInventory>();

        useKey.action.started += ProcessUse;
        interactKey.action.started += ProcessInteract;

        for (int i = 0; i < inventoryKeys.Count; i++)
        {
            InputActionReference action = inventoryKeys[i];

            action.action.started += ProcessInvKey;
        }
    }

    void OnDestroy()
    {
        useKey.action.started -= ProcessUse;
        interactKey.action.started -= ProcessInteract;
        
        for (int i = 0; i < inventoryKeys.Count; i++)
        {
            InputActionReference action = inventoryKeys[i];
            action.action.started -= ProcessInvKey;
        }
    }


    void Start()
    {
        if(unitInventory.Items.Count > 0)
        {
            ItemData item = unitInventory.Items[unitInventory.Items.Count - 1];
            useComponent.SetItemToUse(item);
        }
    }

    private void ProcessUse(InputAction.CallbackContext cnt)
    {
        Vector3 mouseWP = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        mouseWP.z = -1.0f;

        useComponent.Use(mouseWP);
    }
    
    private void ProcessInteract(InputAction.CallbackContext cnt)
    {
        Vector3 mouseWP = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        mouseWP.z = -1.0f;

        Interactable interactable = GetInteractableUnderMouse(mouseWP);

        if(interactable != null)
        {
            if(Vector2.Distance(transform.position, interactable.transform.position) <= interactDist)
            {
                interactable.Interact();
            }
        }
    }

    private void ProcessInvKey(InputAction.CallbackContext cnt)
    {
        int i = 0;
        for(; i < inventoryKeys.Count; i++)
        {
            if(inventoryKeys[i].action == cnt.action)
            {
                break;
            }
        }

        if(i >= inventoryKeys.Count || i >= unitInventory.Items.Count)
        {
            useComponent.SetItemToUse(null);
            return;
        }

        ItemData item = unitInventory.Items[i];
        useComponent.SetItemToUse(item);
    
    }
    private Interactable GetInteractableUnderMouse(Vector3 mousePosition)
    {
        Vector2 worldPoint = mousePosition;
        
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        
        if (hit.collider != null)
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            
            if (interactable == null)
            {
                interactable = hit.collider.GetComponentInParent<Interactable>();
            }
            
            return interactable;
        }
        
        return null;
    }
}
