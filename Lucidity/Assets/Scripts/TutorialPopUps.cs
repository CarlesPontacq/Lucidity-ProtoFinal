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
        GameManager.Instance.OnCameraTaken += ShowCameraPopup;
        cameraManager.OnCameraLookedThroughFirstTime += HideCameraPopup;

        GameManager.Instance.OnReportSheetTaken += ShowReportSheetPopup;
        reportSheetScript.OnReportSheetOpenedFirstTime += HideReportSheetPopup;
        reportSheetScript.OnReportSheetOpenedFirstTime += ShowReportSheetSelectionTutorial;

        // Por ahora en estos eventos, cuando este el otro input del informe se cambian
        reportSheetScript.OnReportSheetOpenedFirstTime += ShowReportSheetSigningTutorial;
        reportSheetScript.OnSignedFirstTime += HideReportSheetSelectionTutorial;
        reportSheetScript.OnSignedFirstTime += HideReportSheetSigningTutorial;
    }

    private void ShowCameraPopup()
    {
        cameraControlsPopUp.SetActive(true);
    }

    private void HideCameraPopup()
    {
        cameraControlsPopUp.SetActive(false);
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

    private void ShowReportSheetSigningTutorial()
    {
        reportSigningTutorial.SetActive(true);
    }

    private void HideReportSheetSigningTutorial()
    {
        reportSigningTutorial.SetActive(false);
    }
}
