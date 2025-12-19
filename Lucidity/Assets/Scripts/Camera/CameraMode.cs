using UnityEngine;

public abstract class CameraMode
{
    public bool isUnlocked;
    public bool isActive = false;

    //Funcion para activar la camara
    public virtual void ActivateMode()
    {
        isActive = true;
        //Poner efectos visuales
    }

    //Funcion para desactivar la camara
    public virtual void DeactivateMode() 
    {
        isActive = false;
        //Quitar efectos visuales
    }
}
