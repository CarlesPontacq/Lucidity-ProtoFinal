using UnityEngine;

public abstract class CameraMode : MonoBehaviour
{
    public bool isUnlocked;
    public bool isActive = false;
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
        //Poner efectos visuales
        OnActivated();
    }

    //Funcion para desactivar la camara
    public virtual void DeactivateMode() 
    {
        isActive = false;
        //Quitar efectos visuales
        OnDeactivated();
    }

    protected virtual void OnActivated() { }

    protected virtual void OnDeactivated() { }
}
