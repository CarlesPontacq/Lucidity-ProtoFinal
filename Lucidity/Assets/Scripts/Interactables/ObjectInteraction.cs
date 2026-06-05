using System;
using Microsoft.Win32.SafeHandles;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    protected bool fullyInteractable = true;
    [SerializeField] protected Transform hintPosition;

    protected virtual void Start() { }

    protected virtual void Update() { }
    public virtual void ResetState() { }

    public virtual void Interact() { }

    public virtual void OnFocusEnter()
    {
        if (!fullyInteractable) return;

        InteractionFeedback.Instance.ShowInteractionFeedback(hintPosition.position);
    }
    
    public virtual void OnFocusExit()
    {
        if(InteractionFeedback.Instance == null) return;
        InteractionFeedback.Instance.HideInteractionFeedback();
    }

    private void OnDestroy()
    {
        OnFocusExit();
    }
}
