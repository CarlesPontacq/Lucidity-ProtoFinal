using UnityEngine;

public class CameraAudioHandler : MonoBehaviour
{
    [Header("Photo")]
    [SerializeField] private string photoSfxId = "cameraShutter";
    [SerializeField] private float photoSfxVolume = 1f;
    [SerializeField] private bool spatialPhotoSfx = false;

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
}
