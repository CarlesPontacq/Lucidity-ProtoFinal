using UnityEngine;

public abstract class CameraMode : MonoBehaviour
{
    [Header("Basic Mode")]
    public bool isUnlocked;
    public bool isActive { get; private set; }

    protected bool unlocked;

    protected void Start()
    {

    }

    protected void Update()
    {
        
    }

    //Funcion para activar la camara
    public virtual void ActivateMode()
    {
        isActive = true;
        OnActivated();
    }

    //Funcion para desactivar la camara
    public virtual void DeactivateMode() 
    {
        isActive = false;
        OnDeactivated();
    }

    public abstract bool PerformCameraAction();

    protected virtual void OnActivated() { }

    protected virtual void OnDeactivated() { }
}
