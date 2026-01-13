using UnityEngine;

public interface ICameraModeListener
{

    void OnCameraModeActivated(CameraMode mode);
    void OnCameraModeDeactivated(CameraMode mode);
}
