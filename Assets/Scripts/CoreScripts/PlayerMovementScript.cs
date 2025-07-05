using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovementScript : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turrentSpeed = 30f;


    // local variables
    private Vector2 previousMovementInput;

    override public void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            // Disable this script for non-owner clients
            return;
        }
        // Subscribe to input events

        inputReader.OnMoveEvent += HandleMovementInput;

    }

    private void HandleMovementInput(Vector2 movementInput)
    {
        previousMovementInput = movementInput;
    }

    override public void OnNetworkDespawn()
    {
        if (!IsOwner) 
        {
            return;
        }

        inputReader.OnMoveEvent -= HandleMovementInput;
    }

    private void Update()
    {
        if(!IsOwner)
        {
            return;
        }

        float zRotation = previousMovementInput.x * turrentSpeed * Time.deltaTime;
        bodyTransform.Rotate(0, 0, -zRotation);
    }


    private void FixedUpdate()
    {
        if(!IsOwner)
        {
            return;
        }

        rb.linearVelocity = (Vector2)bodyTransform.up * previousMovementInput.y * moveSpeed;
    }
}
