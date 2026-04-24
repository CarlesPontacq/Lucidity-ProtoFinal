using UnityEngine;

public class TutorialPopUps : MonoBehaviour
{
    [Header("Popups")]
    [SerializeField] GameObject cameraControlsPopUp;
    [SerializeField] GameObject reportSheetPopUp;
    [SerializeField] GameObject reportSelectionTutorial;
    [SerializeField] GameObject reportSigningTutorial;

    [Header("References")]
    [SerializeField] CameraManager cameraManager;
    [SerializeField] ReportSheetOverlayUI reportSheetScript;

    private void Start()
    {
        GameManager.Instance.OnCameraTaken += CameraTaken;
        GameManager.Instance.OnReportSheetTaken += ReportSheetTaken;
    }

    private void CameraTaken()
    {
        ShowCameraPopup();

        cameraManager.OnCameraLookedThrough += CompleteCameraTutorial;

        reportSheetScript.OnOpened += HideCameraPopup;
        reportSheetScript.OnClosed += ShowCameraPopup;
    }

    private void ReportSheetTaken()
    {
        ShowReportSheetPopup();

        cameraManager.OnCameraLookedThrough += HideReportSheetPopup;
        cameraManager.OnCameraStoppedLookingThrough += ShowReportSheetPopup;

        reportSheetScript.OnOpened += CompleteOpenReportSheetTutorial;
        reportSheetScript.OnNumberSelected += CompleteSelectionTutorial;

        reportSheetScript.OnOpened += ShowReportSheetSelectionTutorial;
        reportSheetScript.OnClosed += HideReportSheetSelectionTutorial;
        
    }

    private void ShowCameraPopup()
    {
        cameraControlsPopUp.SetActive(true);
    }

    private void HideCameraPopup()
    {
        cameraControlsPopUp.SetActive(false);
    }

    private void CompleteOpenReportSheetTutorial()
    {
        HideReportSheetPopup();

        cameraManager.OnCameraLookedThrough -= HideReportSheetPopup;
        cameraManager.OnCameraStoppedLookingThrough -= ShowReportSheetPopup;

        reportSheetScript.OnOpened -= CompleteOpenReportSheetTutorial;
    }

    void CompleteCameraTutorial()
    {
        HideCameraPopup();

        cameraManager.OnCameraLookedThrough -= CompleteCameraTutorial;
        reportSheetScript.OnClosed -= ShowCameraPopup;
    }

    private void ShowReportSheetPopup()
    {
        reportSheetPopUp.SetActive(true);
    }

    private void HideReportSheetPopup()
    {
        reportSheetPopUp.SetActive(false);
    }

    private void ShowReportSheetSelectionTutorial()
    {
        reportSelectionTutorial.SetActive(true);
    }

    private void HideReportSheetSelectionTutorial()
    {
        reportSelectionTutorial.SetActive(false);
    }

    private void CompleteSelectionTutorial()
    {
        HideReportSheetSelectionTutorial();
        ShowReportSheetSigningTutorial();

        reportSheetScript.OnOpened += ShowReportSheetSigningTutorial;
        reportSheetScript.OnClosed += HideReportSheetSigningTutorial;
        reportSheetScript.OnSigned += HideReportSheetSigningTutorial;

        reportSheetScript.OnSigned += CompleteSigningTutorial;

        cameraManager.OnCameraLookedThrough -= HideReportSheetPopup;
        cameraManager.OnCameraStoppedLookingThrough -= ShowReportSheetPopup;

        reportSheetScript.OnOpened -= CompleteOpenReportSheetTutorial;
        reportSheetScript.OnNumberSelected -= CompleteSelectionTutorial;

        reportSheetScript.OnOpened -= ShowReportSheetSelectionTutorial;
        reportSheetScript.OnClosed -= HideReportSheetSelectionTutorial;
    }

    private void ShowReportSheetSigningTutorial()
    {
        reportSigningTutorial.SetActive(true);

    }

    private void HideReportSheetSigningTutorial()
    {
        reportSigningTutorial.SetActive(false);
    }

    private void CompleteSigningTutorial()
    {
        // Por ahora todo junto hasta aplicarlo al nuevo input de numeros
        HideReportSheetSelectionTutorial();
        reportSheetScript.OnOpened -= ShowReportSheetSelectionTutorial;
        reportSheetScript.OnOpened -= ShowReportSheetSigningTutorial;
        reportSheetScript.OnClosed -= HideReportSheetSelectionTutorial;
        reportSheetScript.OnClosed -= HideReportSheetSigningTutorial;
        reportSheetScript.OnSigned -= HideReportSheetSigningTutorial;

        reportSheetScript.OnSigned -= CompleteSigningTutorial;
    }
}
