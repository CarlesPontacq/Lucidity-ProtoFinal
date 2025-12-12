using System.Collections.Generic;
using UnityEngine;

public enum CameraModes
{
    None = 0,
    Documentation = 1,
    Inhibition = 2,
    Ultraviolet = 3,
    Future = 4
}

public class CameraManager : MonoBehaviour
{

    public CameraMode currentMode;
    public List<CameraMode> cameraModes;

    void Start()
    {
        //Se settean todos los modos en la lista del manager 
        cameraModes = new List<CameraMode>();
        /*
         * {
            new DocumentationMode(),
            new InhibitionMode(),
            new UVMode(),
            new FutureMode()
        }
        */
    }

    //Funcion para activar el modo de la camera deseado
    public void SetMode(CameraMode mode)
    {
        DeactivateMode();

        if (mode.isUnlocked)
        {
            currentMode = mode;
            currentMode.ActivateMode();
        }
    }

    //Funcion para desactivar el modo de la camera
    public void DeactivateMode()
    {
        if (currentMode != null)
        {
            currentMode.DeactivateMode();
            currentMode = null;
        }
    }
}
