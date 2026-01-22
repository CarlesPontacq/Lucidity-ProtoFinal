using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Modes")]
    public Transform normalCamera;
    public CameraMode currentMode;
    public List<CameraMode> cameraModes;

    [Header("State")]
    public bool lookingThroughCamera = false;

    [Header("Input")]
    [SerializeField] PlayerInputObserver playerInput;

    [Header("UI")]
    public CameraUIHandler ui;

    [Header("SFX")]
    [SerializeField] private string photoSfxId = "cameraShutter";
    [SerializeField] private float photoSfxVolume = 1f;
    [SerializeField] private bool spatialPhotoSfx = false;

    private bool lastDocOpen = false;

    void Start()
    {
        playerInput.onTakePhoto += OnTryToTakePhoto;
        playerInput.onToggleCamera += OnToggleCamera;

        SetMode(cameraModes[0]);
        ui.ShowReelIndicator(true);
    }

    private void Update()
    {
        bool docOpen = ReportSheetOverlayUI.IsOpen;

        if (docOpen && !lastDocOpen)
        {
            if (lookingThroughCamera)
            {
                StopLookingThroughCamera();
                ui.ShowCameraFlash(false);
            }
        }
        lastDocOpen = docOpen;

        if (docOpen)
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
    }

    void OnTryToTakePhoto()
    {
        if (lookingThroughCamera)
        {
            PerformCameraAction();
        }
    }

    void OnToggleCamera()
    {
        if (!lookingThroughCamera) LookThroughCamera();
        else StopLookingThroughCamera();
    }

    private void PlayPhotoSfx()
    {
        if (string.IsNullOrEmpty(photoSfxId)) return;

        if (SFXManager.Instance != null)
        {
            if (spatialPhotoSfx)
                SFXManager.Instance.PlaySpatialSound(photoSfxId, transform.position, photoSfxVolume);
            else
                SFXManager.Instance.PlayGlobalSound(photoSfxId, photoSfxVolume); 

            return;
        }

        Debug.LogWarning("CameraManager: SFXManager.Instance es null (no se pudo reproducir sonido de foto).");
    }

    private void PerformCameraAction()
    {
        if (currentMode == null) return;

        NotifyModeActivated(currentMode);
        ui.ShowCameraFlash(true);
        bool successfulAction = currentMode.PerformCameraAction();
        if (successfulAction)
            PlayPhotoSfx();
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
                ui.ShowCameraAspect(true);
                break;
            case null:
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
                ui.ShowCameraAspect(false);
                break;
            case null:
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
