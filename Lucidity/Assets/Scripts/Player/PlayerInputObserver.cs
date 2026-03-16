using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputObserver : MonoBehaviour
{
    public enum ActionMap { Player, ReportSheet, ItemInfo, UI }

    [Header("Player Input")]
    [SerializeField] private PlayerInput playerInputRef;

    public Vector2 movement { get; private set; } = Vector2.zero;
    public Vector2 cameraMovement { get; private set; } = Vector2.zero;
    public bool IsPressingRun { get; private set; } = false;
    public Action onRun;

    public Action onInteract;
    public Action onToggleSheet;
    public Action onToggleCamera;
    public Action onTakePhoto;

    public Action onCloseItemInfo;

    public Action onPause;

    [Header("Camera Input")]
    public Action onCameraToggle;
    public Action onCameraAction;
    public Action<int> onChangeCameraMode;

    [Header("Cheats")]
    public Action onCheatNextScene;

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
        movement = new Vector2(Mathf.RoundToInt(movement.x), Mathf.RoundToInt(movement.y));
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        cameraMovement = context.ReadValue<Vector2>();
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

    public void OnChangeCameraMode(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 scroll = context.ReadValue<Vector2>();

        if (Mathf.Abs(scroll.y) < 0.01f) return;

        int direction = scroll.y > 0 ? 1 : -1;
        onChangeCameraMode?.Invoke(direction);
    }

    public void OnCheatNextScene(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
             onCheatNextScene?.Invoke();
        }
    }

    public void OnCloseItemInfo(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            onCloseItemInfo?.Invoke();
    }

    public void SwitchActionMap(ActionMap actionMap)
    {
        playerInputRef.SwitchCurrentActionMap(GetActionMapString(actionMap));
    }

    private string GetActionMapString(ActionMap actionMap)
    {
        string actionMapString = "";
        switch (actionMap)
        {
            case ActionMap.Player:
                actionMapString = "Player";
                break;
            case ActionMap.ReportSheet:
                actionMapString = "ReportSheet";
                break;
            case ActionMap.ItemInfo:
                actionMapString = "ItemInfo";
                break;
            case ActionMap.UI:
                actionMapString = "UI";
                break;
        }
        return actionMapString;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            onPause?.Invoke();
    }
}
