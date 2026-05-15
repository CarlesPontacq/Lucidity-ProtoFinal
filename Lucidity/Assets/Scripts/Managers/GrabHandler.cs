using System;
using UnityEngine;

public class GrabHandler : MonoBehaviour
{
    [Header("Start Setups")]
    public bool startWithCamera = false;
    public bool startWithReportSheet = false;

    [Header("External References")]
    [SerializeField] private CameraRotation cameraRotation;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private CameraFunctionality cameraFunctionality;
    [SerializeField] private ReportSheetOverlayUI reportSheet;
    [SerializeField] private ItemInfoOverlay itemInfoOverlay;
    [SerializeField] private PlayerArmsAnimationController handsWithCamera;
    [SerializeField] private TransitionToAct3 sceneTransition;


    public event Action OnCameraTaken;
    public event Action OnReportSheetTaken;
    public event Action OnStunUnlocked;

    private bool cameraGrabbed = false;
    private bool stunModeUnlockerGrabbed = false;
    private bool reportSheetGrabbed = false;
    private bool gunGrabbed = false;

    void Start()
    {
        SetHandsWithCameraVisibility(false);

        if (startWithCamera)
        {
            cameraGrabbed = true;

            if (cameraFunctionality != null)
                cameraFunctionality.isUnlocked = true;

            if (cameraManager != null)
                cameraManager.SetFunctionality(cameraFunctionality);

            SetHandsWithCameraVisibility(true);
        }

        if (startWithReportSheet)
        {
            reportSheetGrabbed = true;
            reportSheet.Grab();
        }
    }

    public void CameraGrabbed(ItemData itemData)
    {
        cameraGrabbed = true;

        if (cameraFunctionality != null)
            cameraFunctionality.isUnlocked = true;

        if (itemInfoOverlay != null)
            itemInfoOverlay.OpenInfo(itemData);

        if (cameraManager != null)
            cameraManager.SetFunctionality(cameraFunctionality);

        SetHandsWithCameraVisibility(true);

        OnCameraTaken?.Invoke();
    }

    public void ReportSheetGrabbed(ItemData itemData)
    {
        reportSheetGrabbed = true;

        if (reportSheet != null)
            reportSheet.Grab();

        if (itemInfoOverlay != null)
            itemInfoOverlay.OpenInfo(itemData);

        OnReportSheetTaken?.Invoke();
    }

    public void GunGrabbed()
    {
        gunGrabbed = true;
        GameManager.Instance.SetFinishedLoops(true);

        GameManager.CinematicHandlerRef.SetPlayerControlEnabled(false);
        cameraRotation.SetControlEnabled(false);

        StartCoroutine(sceneTransition.PlayTransition());
    }

    public void StunModeUnlockerGrabbed(ItemData itemData)
    {
        stunModeUnlockerGrabbed = true;

        if (cameraManager != null)
            cameraManager.OnGrabbedFlash();

        if (itemInfoOverlay != null)
            itemInfoOverlay.OpenInfo(itemData);

        OnStunUnlocked?.Invoke();
    }


    public void SetHandsWithCameraVisibility(bool visibility)
    {
        if (handsWithCamera == null) return;

        if (visibility)
            handsWithCamera.ShowArms();
        else
            handsWithCamera.HideArms();
    }

    public bool HasUnlockedStun() => stunModeUnlockerGrabbed;
    public bool GetCameraGrabbed() => cameraGrabbed;
    public bool GetReportSheetGrabbed() => reportSheetGrabbed;
}
