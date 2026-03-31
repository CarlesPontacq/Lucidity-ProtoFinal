using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Modes")]
    public CameraMode currentMode;
    public List<CameraMode> cameraModes;
    private int currentModeIndex = 0;

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
    private float lastScrollTime;
    [SerializeField] private float scrollCooldown = 0.15f;


    void Start()
    {
        input.onCameraToggle += HandleCameraToggle;
        input.onCameraAction += HandleCameraAction;
        //input.onChangeCameraMode += HandleChangeCameraMode;
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
        if (currentMode == null || currentMode.isPerformingAction) return;

        currentMode.PerformCameraAction();
    }

    public void SetMode(CameraMode mode)
    {
        if (!lookingThroughCamera || currentMode.isPerformingAction) return;
        if (!mode.isUnlocked) return;

        DeactivateMode();

        GameManager.Instance.SetPlayerControlEnabled(true);
        cameraRotation.SetControlEnabled(true);
        currentMode = mode;
        currentMode.ActivateMode();
    }

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

    private void HandleChangeCameraMode(int direction)
    {
        if (!lookingThroughCamera || currentMode.isPerformingAction) return;
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
        if (currentMode == null || currentMode.isPerformingAction) return;

        currentMode.DeactivateMode();
        currentMode = null;
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

    public void SetStartingCameraMode()
    {
        currentMode = cameraModes[0];
    }
}
