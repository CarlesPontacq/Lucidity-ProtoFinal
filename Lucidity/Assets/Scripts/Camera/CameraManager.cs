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

    [Header("SFX")]
    [SerializeField] private string photoSfxId = "cameraShutter";
    [SerializeField] private float photoSfxVolume = 1f;
    [SerializeField] private bool spatialPhotoSfx = false;

    private bool lastDocOpen = false;

    void Start()
    {
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

        if (lookingThroughCamera)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                PlayPhotoSfx();          
                PerformCameraAction();
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (!lookingThroughCamera) LookThroughCamera();
            else StopLookingThroughCamera();
        }
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
        foreach (var anomaly in FindObjectsByType<Anomaly>(FindObjectsSortMode.None))
            anomaly.OnCameraModeActivated(mode);
    }

    private void NotifyModeDeactivated(CameraMode mode)
    {
        foreach (var anomaly in FindObjectsByType<Anomaly>(FindObjectsSortMode.None))
            anomaly.OnCameraModeDeactivated(mode);
    }
}
