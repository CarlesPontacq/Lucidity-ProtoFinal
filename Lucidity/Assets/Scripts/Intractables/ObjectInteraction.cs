using System;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    protected virtual void Start() { }
    protected virtual void Update() { }

    public virtual void Interact() { }

    public virtual void OnFocusEnter()
    {
        Debug.Log("Seleccionado");
    }
    
    public virtual void OnFocusExit()
    {
        Debug.Log("Deseleccionado");
    }
}
