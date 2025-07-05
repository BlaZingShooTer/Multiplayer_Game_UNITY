using System.IO.Enumeration;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using static PLayerMovement;
using UnityEngine.InputSystem;
using System;


[CreateAssetMenuAttribute(fileName = "New Input Reader", menuName = "ScriptableObjects/InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    private PLayerMovement playerMovement;

    public event Action<bool> OnPrimaryFireEvent;

    public event Action<Vector2> OnMoveEvent;

    public Vector2 AimInput { get; private set; }
    private void OnEnable()
    {
        if(playerMovement == null) 
        {
            playerMovement = new PLayerMovement();
            playerMovement.Player.SetCallbacks(this);
            playerMovement.Player.Enable();
        }
    }

    private void OnDisable()
    {
        if(playerMovement != null)
        {
            playerMovement.Player.Disable();
            playerMovement.Player.SetCallbacks(null);
            playerMovement = null;
        } 
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();

        OnMoveEvent?.Invoke(value);
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
       if(context.performed)
        {
            OnPrimaryFireEvent?.Invoke(true);
        }
        else if(context.canceled)
        {
            OnPrimaryFireEvent?.Invoke(false);
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        AimInput =  context.ReadValue<Vector2>();
    }
}
