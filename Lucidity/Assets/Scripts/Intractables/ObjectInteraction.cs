using System;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    private Material outline;
    private const int OUTLINE_MAT_INDEX = 1;

    protected virtual void Start()
    {
        outline = GetComponent<Renderer>().materials[OUTLINE_MAT_INDEX];
    }

    protected virtual void Update() { }

    public virtual void Interact() { }

    public virtual void OnFocusEnter()
    {
        outline.SetFloat("_Enabled", 1f);

        SFXManager.Instance.PlayGlobalSound("objectHover", 0.5f);
    }
    
    public virtual void OnFocusExit()
    {
        outline.SetFloat("_Enabled", 0f);
    }
}
