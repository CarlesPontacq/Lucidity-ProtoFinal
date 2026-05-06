using System;
using UnityEngine;

[Serializable]
public class TutorialStep
{
    public GameObject popup;

    [NonSerialized] public bool isCompleted;
    [NonSerialized] public bool isVisible;
}

public class TutorialPopUps : MonoBehaviour
{
    [Header("Steps")]
    [SerializeField] private TutorialStep runStep;
    [SerializeField] private TutorialStep cameraStep;
    [SerializeField] private TutorialStep reportSheetStep;
    [SerializeField] private TutorialStep reportSelectionStep;
    [SerializeField] private TutorialStep reportSigningStep;

    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private ReportSheetOverlayUI reportSheet;

    private void Start()
    {
        SetupRunTutorial();
        SetupCameraTutorial();
        SetupReportTutorial();
    }

    private void SetupRunTutorial()
    {
        FirstChaseEnabler.OnFirstChaseStarted += ShowRun;

        playerMovement.OnStartedRunning += CompleteRun;
    }

    private void CompleteRun()
    {
        Complete(runStep);

        playerMovement.OnStartedRunning -= CompleteRun;

        cameraManager.OnCameraLookedThrough -= HideRun;
        cameraManager.OnCameraStoppedLookingThrough -= ShowRun;

        reportSheet.OnOpened -= HideRun;
        reportSheet.OnClosed -= ShowRun;
    }

    private void SetupCameraTutorial()
    {
        GameManager.Instance.OnCameraTaken += StartCameraTutorial;
    }

    private void StartCameraTutorial()
    {
        Show(cameraStep);

        cameraManager.OnCameraLookedThrough += CompleteCamera;

        reportSheet.OnOpened += HideCamera;
        reportSheet.OnClosed += ShowCamera;
    }    

    private void CompleteCamera()
    {
        Complete(cameraStep);

        cameraManager.OnCameraLookedThrough -= CompleteCamera;

        reportSheet.OnOpened -= HideCamera;
        reportSheet.OnClosed -= ShowCamera;
    }

    private void SetupReportTutorial()
    {
        GameManager.Instance.OnReportSheetTaken += StartReportTutorial;
    }

    private void StartReportTutorial()
    {
        Show(reportSheetStep);

        cameraManager.OnCameraLookedThrough += HideReportSheet;
        cameraManager.OnCameraStoppedLookingThrough += ShowReportSheet;

        reportSheet.OnOpened += OpenReportSheet;
        reportSheet.OnClosed += HideSelection;
        reportSheet.OnNumberSelected += SelectNumber;
        reportSheet.OnSigned += CompleteSigning;
    }

    private void OpenReportSheet()
    {
        Complete(reportSheetStep);
        Show(reportSelectionStep);
    }
    private void SelectNumber()
    {
        Complete(reportSelectionStep);
        Show(reportSigningStep);
    }

    private void CompleteSigning()
    {
        Complete(reportSigningStep);
    }

    private void Show(TutorialStep step)
    {
        if (step.isCompleted) return;

        step.popup.SetActive(true);
        step.isVisible = true;
    }

    private void Hide(TutorialStep step)
    {
        if (!step.isVisible) return;

        step.popup.SetActive(false);
        step.isVisible = false;
    }

    private void Complete(TutorialStep step)
    {
        if (step.isCompleted) return;

        Hide(step);
        step.isCompleted = true;
    }

    private void HideRun() => Hide(runStep);
    private void ShowRun() => Show(runStep);

    private void HideCamera() => Hide(cameraStep);
    private void ShowCamera() => Show(cameraStep);

    private void HideReportSheet() => Hide(reportSheetStep);
    private void ShowReportSheet() => Show(reportSheetStep);
    private void HideSelection() => Hide(reportSelectionStep);


}