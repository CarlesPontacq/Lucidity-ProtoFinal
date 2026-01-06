using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputObserver : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInputRef;

    public Vector2 movement { get; private set; } = Vector2.zero;
    public Vector2 cameraMovement { get; private set; } = Vector2.zero;
    public bool IsPressingRun { get; private set; } = false;
    public Action onRun;

    public Action onInteract;

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
        movement = new Vector2(Mathf.RoundToInt(movement.x), Mathf.RoundToInt(movement.y));

        //Debug.Log("Movement: " + movement);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        cameraMovement = context.ReadValue<Vector2>();

        //Debug.Log("Camera: " + cameraMovement);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            onRun?.Invoke();
            IsPressingRun = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            onRun?.Invoke();
            IsPressingRun = false;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            onInteract?.Invoke();
        }
    }
}
