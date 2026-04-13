using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float shiftSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 10f;
    
    [Header("References")]
    public Transform weaponPivot; // Дочерний объект для оружия/прицела
    
    [Header("Rotation Settings")]
    public bool rotatePlayerInstead = false; // Поворачивать всего персонажа
    public float rotationSpeed = 360f;
    
    private Vector2 currentVelocity;
    private Vector2 targetVelocity;
    private Rigidbody2D rb;
    private Camera mainCamera;

    private float currentSpeed;

    void Awake()
    {
        currentSpeed = moveSpeed;

        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    public void SetRun(bool isRunning)
    {
        if(isRunning)
        {
            currentSpeed = shiftSpeed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }
    }

    public void SetMoveInput(Vector2 dir)
    {
        targetVelocity = dir.normalized;
    }

    public void SetRotation(float angle)
    {
        weaponPivot.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    public Vector3 GetForwardDir()
    {
        return weaponPivot.up;
    }

    public Vector3 GetRightDir()
    {
        return weaponPivot.right;
    }
    
    void FixedUpdate()
    {
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity * currentSpeed, 
            ref currentVelocity, 
            targetVelocity.magnitude > 0 ? 1f/acceleration : 1f/deceleration);
    }
}