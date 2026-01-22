using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputObserver : MonoBehaviour
{
    [Header("Player Input")]
    [SerializeField] private PlayerInput playerInputRef;

    public Vector2 movement { get; private set; } = Vector2.zero;
    public Vector2 cameraMovement { get; private set; } = Vector2.zero;
    public bool IsPressingRun { get; private set; } = false;
    public Action onRun;

    public Action onInteract;

    public Action onToggleSheet;

    [Header("Camera Input")]
    public Action onCameraToggle;
    public Action onCameraAction;
    public Action onSetDocumentationMode;
    public Action onSetUltravioletMode;

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

    public void OnToggleSheet(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            onToggleSheet?.Invoke();
        }
    }

    public void OnCameraToggle(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            onCameraToggle?.Invoke();
    }

    public void OnCameraAction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            onCameraAction?.Invoke();
    }

    public void OnSetDocumentationMode(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            onSetDocumentationMode?.Invoke();
    }
    public void OnSetUltravioletMode(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            onSetUltravioletMode?.Invoke();
    }
}
