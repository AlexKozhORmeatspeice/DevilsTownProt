using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerContoller : MonoBehaviour
{
    [SerializeField] private UnitMovement unitMovement;
    
    [Header("Input")]
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference startRun;

    private Vector3 moveDir;
    private float lastTimeShoot = -999f;



    public void Start()
    {
        startRun.action.started += OnStartedRun;
        startRun.action.canceled += OnEndedRun;
    }

    void OnDisable()
    {
        startRun.action.started -= OnStartedRun;
        startRun.action.canceled -= OnEndedRun;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        RotatePlayerToMouse();
    }

    private void HandleMovementInput()
    {
        moveDir = move.action.ReadValue<Vector2>();

        unitMovement.SetMoveInput(moveDir);
    }

    private void RotatePlayerToMouse()
    {
        if(Camera.main == null) return;
        
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = transform.position.z;
        
        Vector2 direction = mousePosition - transform.position;
        
        if (direction.magnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            // Плавный поворот
            unitMovement.SetRotation(angle);
        }
    }


    private void OnStartedRun(InputAction.CallbackContext cnt)
    {
        unitMovement.SetRun(true);
    }

    private void OnEndedRun(InputAction.CallbackContext cnt)
    {
        unitMovement.SetRun(false);
    }
}
