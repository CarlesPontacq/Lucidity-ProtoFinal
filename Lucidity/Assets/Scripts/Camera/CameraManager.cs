using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private bool lastDocOpen = false;

    void Start()
    {
        SetMode(cameraModes[0]);
        ui.ShowReelIndicator(true);
    }

    private void Update()
    {
        bool docOpen = ReportSheetOverlayUI.IsOpen;

        //Si se acaba de abrir el documento, sale de la camara automaticamente
        if (docOpen && !lastDocOpen)
        {
            if (lookingThroughCamera)
            {
                StopLookingThroughCamera();
                ui.ShowCameraFlash(false);
            }
        }
        lastDocOpen = docOpen;

        //Mientras el documento este abierto, no proceses inputs de camara
        if (docOpen)
            return;

        //Si el puntero esta encima de UI, ignora clicks del mundo/camara
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (lookingThroughCamera)
        {
            // Hacer el Flash / acción de cámara
            if (Input.GetButtonDown("Fire2"))
            {
                PerformCameraAction();
            }
        }

        // Poner/Usar la vista de cámara
        if (Input.GetButtonDown("Fire1"))
        {
            if (!lookingThroughCamera)
            {
                LookThroughCamera();
            }
            else
            {
                StopLookingThroughCamera();
            }
        }
    }

    // Funcion para hacer la acción correspondiente de la camara seleccionada
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

    // Funcion para activar el modo de la camera deseado
    public void SetMode(CameraMode mode)
    {
        DeactivateMode();

        if (!mode.isUnlocked) return;

        currentMode = mode;
    }

    // Funcion para desactivar el modo de la camera
    public void DeactivateMode()
    {
        if (currentMode == null) return;

        NotifyModeDeactivated(currentMode);
        StopLookingThroughCamera();
        currentMode.DeactivateMode();
        currentMode = null;
    }

    // Funcion para mirar/usar la camara
    private void LookThroughCamera()
    {
        if (currentMode == null) return;

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

    // Funcion para dejar de mirar/usar la camara
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

    // Funcion para avisar a las anomalias de lo que tengan que hacer cuando se activa el modo de camera
    private void NotifyModeActivated(CameraMode mode)
    {
        foreach (var anomaly in FindObjectsByType<Anomaly>(FindObjectsSortMode.None))
            anomaly.OnCameraModeActivated(mode);
    }

    // Funcion para avisar a las anomalias de lo que tengan que hacer cuando se desactiva el modo de camera
    private void NotifyModeDeactivated(CameraMode mode)
    {
        foreach (var anomaly in FindObjectsByType<Anomaly>(FindObjectsSortMode.None))
            anomaly.OnCameraModeDeactivated(mode);
    }
}
