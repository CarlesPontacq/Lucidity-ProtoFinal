using NUnit.Framework;
using UnityEngine;

public class CameraObject : ObjectInteraction
{
    [SerializeField] ItemData itemData;
    [SerializeField] private string grabCameraSFX = "GrabCamera";
    private float grabCameraVolumeSFX = 1f;

    public override void Interact()
    {
        SFXManager.Instance.PlayGlobalSound(grabCameraSFX, grabCameraVolumeSFX);
        GameManager.Instance.CameraGrabbed(itemData);
        Destroy(gameObject);
    }
}
