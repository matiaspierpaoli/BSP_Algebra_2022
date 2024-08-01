using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public event Action InitializePointsEvent;
    public event Action CalculatePointRoomsEvent;
    public event Action ResetPointsEvent;
    public event Action DebugPlayerEvent;

    public void OnInitializePoints(InputAction.CallbackContext context)
    {
        if (context.performed)
            InitializePointsEvent?.Invoke();
    }

    public void OnCalculatePointRooms(InputAction.CallbackContext context)
    {
        if (context.performed)
            CalculatePointRoomsEvent?.Invoke();
    }
    
    public void OnResetRooms(InputAction.CallbackContext context)
    {
        if (context.performed)
            ResetPointsEvent?.Invoke();
    }
    
    public void OnDebugPlayer(InputAction.CallbackContext context)
    {
        if (context.performed)
            DebugPlayerEvent?.Invoke();
    }

}
