using System;
using Microsoft.Win32.SafeHandles;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    protected virtual void Start() { }

    protected virtual void Update() { }

    public virtual void Interact() { }

    public virtual void OnFocusEnter()
    {
        ReticleController.Instance.MakeReticleBigger();
        SFXManager.Instance.PlayGlobalSound("objectHover", 0.5f);
    }
    
    public virtual void OnFocusExit()
    {
        ReticleController.Instance.ReturnReticleToNormalSize();
    }
}
