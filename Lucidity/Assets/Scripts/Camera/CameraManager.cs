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

    [Header("State")]
    public bool lookingThroughCamera = false;

    [Header("UI")]
    public CameraUIHandler ui;

    private bool lastDocOpen = false;

    [Header("Input")]
    [SerializeField] private PlayerInputObserver input;
    private float lastScrollTime;
    [SerializeField] private float scrollCooldown = 0.15f;


    void Start()
    {
        input.onCameraToggle += HandleCameraToggle;
        input.onCameraAction += HandleCameraAction;
        input.onChangeCameraMode += HandleChangeCameraMode;
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

    private void PerformCameraAction()
    {
        if (currentMode == null) return;

        currentMode.PerformCameraAction();
    }

    public void SetMode(CameraMode mode)
    {
        if (!lookingThroughCamera) return;
        if (!mode.isUnlocked) return;
        
        DeactivateMode();
        
        currentMode = mode;
        currentMode.ActivateMode();

        ui.SetIndicatorPosition(cameraModes.IndexOf(mode));
    }

    private void HandleCameraToggle()
    {
        if (ReportSheetOverlayUI.IsOpen) return;

        if (!lookingThroughCamera)
            LookThroughCamera();
        else
            StopLookingThroughCamera();

        GameManager.Instance.SetHandsWithCameraVisibility(!lookingThroughCamera);
    }

    private void HandleCameraAction()
    {
        if (!lookingThroughCamera || currentMode == null) return;

        PerformCameraAction();
    }

    private void HandleChangeCameraMode(int direction)
    {
        if (!lookingThroughCamera) return;
        if (cameraModes == null || cameraModes.Count == 0) return;

        if (Time.time - lastScrollTime < scrollCooldown) return;
        lastScrollTime = Time.time;

        int startIndex = currentModeIndex;
        int index = currentModeIndex;

        do
        {
            index = (index + direction + cameraModes.Count) % cameraModes.Count;

            if (cameraModes[index].isUnlocked)
            {
                currentModeIndex = index;
                SetMode(cameraModes[currentModeIndex]);
                return;
            }

        } while (index != startIndex);
    }

    public void DeactivateMode()
    {
        if (currentMode == null) return;

        //StopLookingThroughCamera();
        currentMode.DeactivateMode();
        currentMode = null;
    }

    private void LookThroughCamera()
    {
        if (currentMode == null) return;

        lookingThroughCamera = true;
        currentMode.ActivateMode();

        ui.ShowCameraAspect(true);
    }

    private void StopLookingThroughCamera()
    {
        if (currentMode == null) return;

        lookingThroughCamera = false;
        currentMode.DeactivateMode();

        ui.ShowCameraAspect(false);
    }

    public void SetStartingCameraMode()
    {
        currentMode = cameraModes[0];
    }
}
