using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraPostProcessToggle : MonoBehaviour
{
    public Camera playerCamera;
    public Camera armsCamera;

    private UniversalAdditionalCameraData playerData;
    private UniversalAdditionalCameraData armsData;

    void Start()
    {
        playerData = playerCamera.GetUniversalAdditionalCameraData();
        armsData = armsCamera.GetUniversalAdditionalCameraData();
    }

    public void ToggleCameraPostProcessing(bool value)
    {
        playerData.renderPostProcessing = value;
        armsData.renderPostProcessing = !value;
    }
}
