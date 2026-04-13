using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Action onInteract;

    public void Interact()
    {
        onInteract?.Invoke();
    }
}
