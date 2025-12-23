using System.Collections.Generic;
using UnityEngine;


public class CameraManager : MonoBehaviour
{
    public Transform normalCamera;
    public Transform currentActiveCamera;
    public CameraMode currentMode;
    public List<CameraMode> cameraModes;

    void Start()
    {

    }

    private void Update()
    {
        //Testeo para probar que funcione
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            SetMode(cameraModes[0]);
            currentActiveCamera = cameraModes[0].transform;
            normalCamera.GetComponent<Camera>().enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            normalCamera.GetComponent<Camera>().enabled = true;
            currentActiveCamera = normalCamera;
            DeactivateMode();
        }
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
