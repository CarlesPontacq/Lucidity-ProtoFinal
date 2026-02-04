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
        UIManager.Instance.MakeReticleBigger();
        SFXManager.Instance.PlayGlobalSound("objectHover", 0.5f);
    }
    
    public virtual void OnFocusExit()
    {
        UIManager.Instance.ReturnReticleToNormalSize();
    }
}
