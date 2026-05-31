using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CinematicInputObserver : MonoBehaviour
{
    public Action onSkipCinematic;

    [SerializeField] private InputActionReference skipActionRef;

    private InputAction action;

    private void Awake()
    {
        action = skipActionRef.action;
    }

    public void OnSkipCinematic(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onSkipCinematic?.Invoke();
        }
    }

    public float GetHoldProgress()
    {
        return action.GetTimeoutCompletionPercentage();
    }
}