using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class CameraManager : MonoBehaviour
{
    [Header("CameraFunctions")]
    [SerializeField] private CameraMode currentMode;
    public bool hasFlashCamera = false;

    [Header("State")]
    public bool lookingThroughCamera = false;
    [SerializeField] private CameraPostProcessToggle cameraPostProcessToggle;
    [SerializeField] private CameraRotation cameraRotation;
    [SerializeField] private CameraAudioHandler audioHandler;


    [Header("UI")]
    public CameraUIHandler ui;
    private bool lastDocOpen = false;

    [Header("Input")]
    [SerializeField] private PlayerInputObserver input;

    void Start()
    {
        input.onCameraToggle += HandleCameraToggle;
        input.onCameraAction += HandleCameraAction;
    }

    //Needs to change currentMode to whatever it will end up being

    #region Input
    private void HandleCameraToggle()
    {
        if (currentMode == null) return;

        if (ReportSheetOverlayUI.IsOpen || currentMode.isPerformingAction) return;

        if (!lookingThroughCamera)
            LookThroughCamera();
        else
            StopLookingThroughCamera();

        GameManager.Instance.SetHandsWithCameraVisibility(!lookingThroughCamera);
    }

    private void HandleCameraAction()
    {
        if (!lookingThroughCamera || currentMode == null) return;
        if (currentMode.isPerformingAction) return;

        PerformCameraAction();
    }
    #endregion

    #region Actions
    private void PerformCameraAction()
    {
        if (currentMode == null || currentMode.isPerformingAction || !hasFlashCamera) return;

        currentMode.PerformCameraAction();
    }

    private void LookThroughCamera()
    {
        if (currentMode == null) return;

        lookingThroughCamera = true;
        currentMode.ActivateMode();

        ui.ShowCameraAspect(true);
        InteractionFeedback.Instance.ShowInteractHint(false);
    }

    private void StopLookingThroughCamera()
    {
        if (currentMode == null || currentMode.isPerformingAction) return;

        lookingThroughCamera = false;
        currentMode.DeactivateMode();

        ui.ShowCameraAspect(false);
    }
    #endregion


}
