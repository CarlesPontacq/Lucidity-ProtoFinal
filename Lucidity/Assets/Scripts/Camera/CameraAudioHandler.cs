using UnityEngine;

public class CameraAudioHandler : MonoBehaviour
{
    [Header("Photo")]
    [SerializeField] private string photoSfxId = "cameraShutter";
    [SerializeField] private float photoSfxVolume = 1f;
    [SerializeField] private bool spatialPhotoSfx = false;

    [Header("Mode")]
    [SerializeField] private string modeSfxId = "changeCameraMode";
    [SerializeField] private float modeSfxVolume = 1f;

    public void PlayPhotoSfx()
    {
        if (string.IsNullOrEmpty(photoSfxId)) return;

        if (SFXManager.Instance != null)
        {
            if (spatialPhotoSfx)
                SFXManager.Instance.PlaySpatialSound(photoSfxId, transform.position, photoSfxVolume);
            else
                SFXManager.Instance.PlaySpatialSound(photoSfxId, transform.position, photoSfxVolume);
            return;
        }
    }

    public void PlayChangeCameraModeSfx()
    {
        if (string.IsNullOrEmpty(modeSfxId)) return;

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySpatialSound(modeSfxId, transform.position, modeSfxVolume);
        }
    }
}
