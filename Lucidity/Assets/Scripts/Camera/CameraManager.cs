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
    private float lastScrollTime;
    [SerializeField] private float scrollCooldown = 0.15f;


    void Start()
    {
        input.onCameraToggle += HandleCameraToggle;
        input.onCameraAction += HandleCameraAction;
        //input.onSetDocumentationMode += HandleSetDocumentationMode;
        //input.onSetUltravioletMode += HandleSetUltravioletMode;
        input.onChangeCameraMode += HandleChangeCameraMode;

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

    /*
    private void HandleSetDocumentationMode()
    {
        if (cameraModes == null || cameraModes.Count == 0) return;

        int newcurrentModeIndex = documentationModeIndex;

        if (!cameraModes[newcurrentModeIndex].isUnlocked) return;

        currentModeIndex = newcurrentModeIndex;

        SetMode(cameraModes[currentModeIndex]);
    }
    */

    /*
    private void HandleSetUltravioletMode()
    {
        if (cameraModes == null || cameraModes.Count == 0) return;

        int newcurrentModeIndex = ultravioletModeIndex;

        if (!cameraModes[newcurrentModeIndex].isUnlocked) return;

        currentModeIndex = newcurrentModeIndex;

        SetMode(cameraModes[currentModeIndex]);
    }
    */

    private void HandleChangeCameraMode(int direction)
    {
        if (lookingThroughCamera) return;
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

        StopLookingThroughCamera();
        currentMode.DeactivateMode();
        currentMode = null;
    }

    private void LookThroughCamera()
    {
        if (currentMode == null) return;

        currentMode.ActivateMode();

        lookingThroughCamera = true;

        currentMode.LookThroughCamera(lookingThroughCamera);
    }

    private void StopLookingThroughCamera()
    {
        if (currentMode == null) return;

        lookingThroughCamera = false;

        currentMode.LookThroughCamera(lookingThroughCamera);
    }
}
