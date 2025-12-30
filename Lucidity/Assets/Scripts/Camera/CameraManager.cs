using System;
using System.Collections.Generic;
using UnityEngine;


public class CameraManager : MonoBehaviour
{
    [Header("Camera Modes")]
    public Transform normalCamera;
    public CameraMode currentMode;

    public List<CameraMode> cameraModes;

    [Header("State")]
    public bool lookingThroughCamera = false;

    [Header("UI")]
    public CameraUIHandler ui;

    void Start()
    {
        ui.ShowReelIndicator(lookingThroughCamera);
    }

    private void Update()
    {
        if (lookingThroughCamera)
        {
            //Hacer el Flash
            if (Input.GetButtonDown("Fire2"))
            {
                if (lookingThroughCamera)
                {
                    PerformCameraAction();
                }
            }
        }
        else
        {
            //Testeo de no tener ningun modo seleccionado
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                DeactivateMode();
                ui.ShowReelIndicator(false);
            }
        }

        //Seleccionar el primer modo
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            SetMode(cameraModes[0]);
            ui.ShowReelIndicator(true);
        }

        //Poner/Usar la vista de camara camara
        if (Input.GetButtonDown("Fire1"))
        {
            if(!lookingThroughCamera)
            {
                LookThroughCamera();
            }
            else
            {
                StopLookingThroughCamera();
            }
        }
    }

    //Funcion para hacer la acción correspondiente de la camara seleccionada
    private void PerformCameraAction()
    {
        if (currentMode == null) return;

        NotifyModeActivated(currentMode);
        ui.ShowCameraFlash(true);
        currentMode.PerformCameraAction();
    }

    public void EndCameraAction()
    {
        if (currentMode == null) return;

        NotifyModeDeactivated(currentMode);
        ui.ShowCameraFlash(false);
    }

    //Funcion para activar el modo de la camera deseado
    public void SetMode(CameraMode mode)
    {
        DeactivateMode();

        if (!mode.isUnlocked) return;

        currentMode = mode;
    }

    //Funcion para desactivar el modo de la camera
    public void DeactivateMode()
    {
        if (currentMode == null) return;
        
        NotifyModeDeactivated(currentMode);
        StopLookingThroughCamera();
        currentMode.DeactivateMode();
        currentMode = null;
       
    }

    //Funcion para mirar/usar la camara
    private void LookThroughCamera()
    {
        if(currentMode == null) return;

        currentMode.ActivateMode();

        switch (currentMode)
        {
            case DocumentationMode:
                ui.ShowCameraAspect(true);
            break;
            case null:
                Debug.Log("Null Camera Mode");
            break;
        }

        lookingThroughCamera = true;
    }

    //Funcion para dejar de mirar/usar la camara
    private void StopLookingThroughCamera()
    {
        if (currentMode == null) return;

        switch (currentMode)
        {
            case DocumentationMode:
                ui.ShowCameraAspect(false);
                break;
            case null:
                Debug.Log("Null Camera Mode");
                break;
        }

        lookingThroughCamera = false;
    }

    //Funcion para avisar a las anomalias de lo que tengan que hacer cuando se activa el modo de camera
    private void NotifyModeActivated(CameraMode mode)
    {
        foreach (var anomaly in FindObjectsOfType<Anomaly>())
            anomaly.OnCameraModeActivated(mode);
    }

    //Funcion para avisar a las anomalias de lo que tengan que hacer cuando se desactiva el modo de camera
    private void NotifyModeDeactivated(CameraMode mode)
    {
        foreach (var anomaly in FindObjectsOfType<Anomaly>())
            anomaly.OnCameraModeDeactivated(mode);
    }
}
