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
    private int currentModeIndex = 0;

    private int documentationModeIndex = 0;
    private int ultravioletModeIndex = 1;

    [Header("State")]
    public bool lookingThroughCamera = false;

    [Header("UI")]
    public CameraUIHandler ui;

    private bool lastDocOpen = false;

    [Header("Input")]
    [SerializeField] private PlayerInputObserver input;


    void Start()
    {
        input.onCameraToggle += HandleCameraToggle;
        input.onCameraAction += HandleCameraAction;
        input.onSetDocumentationMode += HandleSetDocumentationMode;
        input.onSetUltravioletMode += HandleSetUltravioletMode;

        SetMode(cameraModes[0]);
    }

    private void Update()
    {
        bool docOpen = ReportSheetOverlayUI.IsOpen;

        if (docOpen && !lastDocOpen && lookingThroughCamera)
        {
            StopLookingThroughCamera();
            ui.ShowCameraFlash(false);
        }

        lastDocOpen = docOpen;
    }

    private void HandleCameraToggle()
    {
        if (ReportSheetOverlayUI.IsOpen) return;

        if (!lookingThroughCamera)
            LookThroughCamera();
        else
            StopLookingThroughCamera();
    }

    private void HandleCameraAction()
    {
        if (!lookingThroughCamera || currentMode == null) return;

        PerformCameraAction();
    }

    private void HandleSetDocumentationMode()
    {
        if (cameraModes == null || cameraModes.Count == 0) return;

        int newcurrentModeIndex = documentationModeIndex;

        if (!cameraModes[newcurrentModeIndex].isUnlocked) return;

        currentModeIndex = newcurrentModeIndex;

        SetMode(cameraModes[currentModeIndex]);
    }

    private void HandleSetUltravioletMode()
    {
        if (cameraModes == null || cameraModes.Count == 0) return;

        int newcurrentModeIndex = ultravioletModeIndex;

        if (!cameraModes[newcurrentModeIndex].isUnlocked) return;

        currentModeIndex = newcurrentModeIndex;

        SetMode(cameraModes[currentModeIndex]);
    }

    private void PerformCameraAction()
    {
        if (currentMode == null) return;

        currentMode.PerformCameraAction();
    }

    public void SetMode(CameraMode mode)
    {
        if (lookingThroughCamera) return;
        DeactivateMode();
        if (!mode.isUnlocked) return;
        currentMode = mode;
        ui.SetCameraModeUI(currentMode);
    }

    public void DeactivateMode()
    {
        if (currentMode == null) return;

        StopLookingThroughCamera();
        currentMode.DeactivateMode();
        currentMode = null;
    }

    private void LookThroughCamera() //<- No debería depender del modo de la cámara, seguramente con el nuevo ui se solucione o medio solucione
    {
        if (currentMode == null) return;

        currentMode.ActivateMode();

        switch (currentMode)
        {
            case DocumentationMode:
                ui.ShowDocumentationCameraAspect(true);
                break;
            case UltravioletMode:
                ui.ShowUvCameraAspect(true);
                currentMode.PerformCameraAction();
                break;
            default:
                Debug.Log("Null Camera Mode");
                break;
        }

        lookingThroughCamera = true;
    }

    private void StopLookingThroughCamera() //<- No debería depender del modo de la cámara, seguramente con el nuevo ui se solucione o medio solucione
    {
        if (currentMode == null) return;

        switch (currentMode)
        {
            case DocumentationMode:
                ui.ShowDocumentationCameraAspect(false);
                break;
            case UltravioletMode:
                ui.ShowUvCameraAspect(false);
                UltravioletMode uvMode = currentMode as UltravioletMode;
                uvMode.isUvLightOn = false;
                break;
            default:
                Debug.Log("Null Camera Mode");
                break;
        }

        lookingThroughCamera = false;
    }
}
