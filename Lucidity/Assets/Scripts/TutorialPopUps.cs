using UnityEngine;

public class TutorialPopUps : MonoBehaviour
{
    [SerializeField] GameObject cameraControlsPopUp;
    [SerializeField] GameObject reportSheetPopUp;

    [SerializeField] CameraManager cameraManager;
    [SerializeField] ReportSheetOverlayUI reportSheetScript;
    private void Start()
    {
        GameManager.Instance.OnCameraTaken += ShowCameraPopup;
        cameraManager.OnCameraLookedThroughFirstTime += HideCameraPopup;

        GameManager.Instance.OnReportSheetTaken += ShowReportSheetPopup;
        reportSheetScript.OnReportSheetOpenedFirstTime += HideReportSheetPopup;
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
}
