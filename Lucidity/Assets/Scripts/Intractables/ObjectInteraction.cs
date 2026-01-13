using System;
using Microsoft.Win32.SafeHandles;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    private Vector3 originalScale;
    private float scaleMultiplier = 1.05f;

    protected virtual void Start()
    {
        originalScale = transform.localScale;

        Transform outlineTransform = transform.parent.Find("Outline");
        if (outlineTransform == null)
        {
            Debug.LogError("Falta el Outline como hermano de: " + gameObject.name);
            return;
        }

        MeshRenderer outlineRenderer = outlineTransform.GetComponent<MeshRenderer>();
        if (outlineRenderer == null)
        {
            Debug.LogError("Falta renderer del outline: " + gameObject.name);
            return;
        }
    }

    protected virtual void Update() { }

    public virtual void Interact() { }

    public virtual void OnFocusEnter()
    {
        UIManager.Instance.MakeReticleBigger();
        transform.localScale = originalScale * scaleMultiplier;

        SFXManager.Instance.PlayGlobalSound("objectHover", 0.5f);
    }
    
    public virtual void OnFocusExit()
    {
        UIManager.Instance.ReturnReticleToNormalSize();
        transform.localScale = originalScale;
    }
}
