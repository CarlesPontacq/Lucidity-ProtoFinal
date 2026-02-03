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

    [Header("SFX")]
    [SerializeField] private string photoSfxId = "cameraShutter";
    [SerializeField] private float photoSfxVolume = 1f;
    [SerializeField] private bool spatialPhotoSfx = false;

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
        //ui.ShowReelIndicator(true);
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

        if (currentMode == cameraModes[ultravioletModeIndex])
        {
            UltravioletMode uv = currentMode as UltravioletMode;

            if (uv.isUvLightOn) return;
        }

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

    private void PlayPhotoSfx()
    {
        if (string.IsNullOrEmpty(photoSfxId)) return;

        if (SFXManager.Instance != null)
        {
            if (spatialPhotoSfx)
                SFXManager.Instance.PlaySpatialSound(photoSfxId, transform.position, photoSfxVolume);
            else
                SFXManager.Instance.PlaySpatialSound(photoSfxId, transform.position, photoSfxVolume);

            return;
        }

        Debug.LogWarning("CameraManager: SFXManager.Instance es null (no se pudo reproducir sonido de foto).");
    }

    private void PerformCameraAction()
    {
        if (currentMode == null) return;

        if (currentMode == cameraModes[documentationModeIndex])
        {
            NotifyModeActivated(currentMode);
            ui.ShowCameraFlash(true);
        }

        bool successfulCameraAction = currentMode.PerformCameraAction();
        if (successfulCameraAction && currentMode == cameraModes[documentationModeIndex])
        {
            PlayPhotoSfx();
        }
    }

    public void EndCameraAction()
    {
        if (currentMode == null) return;

        NotifyModeDeactivated(currentMode);
        ui.ShowCameraFlash(false);
    }

    public void SetMode(CameraMode mode)
    {
        DeactivateMode();
        if (!mode.isUnlocked) return;
        currentMode = mode;
        ui.SetCameraModeUI(currentMode);
    }

    public void DeactivateMode()
    {
        if (currentMode == null) return;

        NotifyModeDeactivated(currentMode);
        StopLookingThroughCamera();
        currentMode.DeactivateMode();
        currentMode = null;
    }

    private void LookThroughCamera()
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
                break;
            default:
                Debug.Log("Null Camera Mode");
                break;
        }

        lookingThroughCamera = true;
    }

    private void StopLookingThroughCamera()
    {
        if (currentMode == null) return;

        switch (currentMode)
        {
            case DocumentationMode:
                ui.ShowDocumentationCameraAspect(false);
                break;
            case UltravioletMode:
                ui.ShowUvCameraAspect(false);
                break;
            default:
                Debug.Log("Null Camera Mode");
                break;
        }

        lookingThroughCamera = false;
    }

    private void NotifyModeActivated(CameraMode mode)
    {
        var manager = FindAnyObjectByType<AnomalyManager>();
        if (manager == null) return;

        foreach (var anomaly in manager.GetSpawnedEnemiesThisLoop())
            anomaly.OnCameraModeActivated(mode);
    }

    private void NotifyModeDeactivated(CameraMode mode)
    {
        var manager = FindAnyObjectByType<AnomalyManager>();
        if (manager == null) return;

        foreach (var anomaly in manager.GetSpawnedEnemiesThisLoop())
            anomaly.OnCameraModeDeactivated(mode);
    }
}
