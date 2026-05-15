using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class CameraManager : MonoBehaviour
{
    [Header("CameraFunctions")]
    [SerializeField] private CameraFunctionality functionality;
    [SerializeField] private bool hasFlashCamera = false;

    [SerializeField] private GameObject flashComponent;

    [Header("State")]
    public bool lookingThroughCamera = false;
    [SerializeField] private CameraPostProcessToggle cameraPostProcessToggle;
    [SerializeField] private CameraRotation cameraRotation;

    public event Action OnCameraLookedThrough;
    public event Action OnCameraStoppedLookingThrough;

    [SerializeField] private CameraAudioHandler audioHandler;

    private bool isTransitioning = false;
    [SerializeField] private PlayerArmsAnimationController armsController;

    [Header("UI")]
    public CameraUIHandler ui;
    private bool lastDocOpen = false;

    [Header("Input")]
    [SerializeField] private PlayerInputObserver input;

    void Start()
    {
        input.onCameraToggle += HandleCameraToggle;
        input.onCameraAction += HandleCameraPhoto;

        if(!hasFlashCamera)
            flashComponent.SetActive(false);
    }

    private void Update()
    {

        bool docOpen = ReportSheetOverlayUI.IsOpen;

        if ((docOpen && !lastDocOpen && lookingThroughCamera) || GameManager.DeathHandlerRef.isDying)
        {
            StopLookingThroughCamera();
            ui.ShowCameraFlash(false);
        }

        lastDocOpen = docOpen;
    }

    public void SetFunctionality(CameraFunctionality newfunctionality)
    {
        functionality = newfunctionality;
    }

    public void OnGrabbedFlash()
    {
        hasFlashCamera = true;
        flashComponent.SetActive(true);
    }

    #region Input
    private void HandleCameraToggle()
    {
        if (functionality == null) return;
        if (ReportSheetOverlayUI.IsOpen || functionality.isPerformingAction) return;
        if (isTransitioning || GameManager.DeathHandlerRef.isDying) return;

        isTransitioning = true;
        if (!lookingThroughCamera)
        {
            if (armsController != null)
                armsController.PlayRaiseCamera();
            else
                LookThroughCamera();
        }
        else
        {
            if (armsController != null)
                armsController.PlayLowerCamera();
            else
                StopLookingThroughCamera();
        }
    }

    private void HandleCameraPhoto()
    {
        if (!lookingThroughCamera || functionality == null) return;
        if (functionality.isPerformingAction) return;

        PerformCameraAction();
    }
    #endregion

    #region Actions
    private void PerformCameraAction()
    {
        if (functionality == null || functionality.isPerformingAction || !hasFlashCamera) return;
        functionality.PerformCameraPhoto();
    }

    private void LookThroughCamera()
    {
        if (functionality == null) return;

        lookingThroughCamera = true;
        functionality.ActivateMode();

        OnCameraLookedThrough?.Invoke();

        ui.ShowCameraAspect(true);
        InteractionFeedback.Instance.HideInteractHint();

        GameManager.GrabHandlerRef.SetHandsWithCameraVisibility(!lookingThroughCamera);
        isTransitioning = false;
    }

    private void StopLookingThroughCamera()
    {
        if (functionality == null || functionality.isPerformingAction) return;

        lookingThroughCamera = false;
        functionality.DeactivateMode();

        OnCameraStoppedLookingThrough?.Invoke();

        ui.ShowCameraAspect(false);

        GameManager.GrabHandlerRef.SetHandsWithCameraVisibility(!lookingThroughCamera);
        isTransitioning = false;
    }
    #endregion

    #region AnimationEvents
    public void OnCameraRaised()
    {
        LookThroughCamera();
    }

    public void OnCameraLowered()
    {
        StopLookingThroughCamera();
    }
    #endregion
}
