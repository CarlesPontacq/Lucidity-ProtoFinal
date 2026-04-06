using System;
using Microsoft.Win32.SafeHandles;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    protected bool fullyInteractable = true;

    protected virtual void Start() { }

    protected virtual void Update() { }

    public virtual void Interact() { }

    public virtual void OnFocusEnter()
    {
        if (!fullyInteractable) return;

        InteractionFeedback.Instance.ShowInteractionFeedback();
    }
    
    public virtual void OnFocusExit()
    {
        InteractionFeedback.Instance.HideInteractionFeedback();
    }

    private void OnDestroy()
    {
        OnFocusExit();
    }
}
