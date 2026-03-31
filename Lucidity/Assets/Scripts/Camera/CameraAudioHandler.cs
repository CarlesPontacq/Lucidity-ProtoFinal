using UnityEngine;

public class CameraAudioHandler : MonoBehaviour
{
    [Header("Photo")]
    [SerializeField] private string photoSfxId = "cameraShutter";
    [SerializeField] private float photoSfxVolume = 1f;

    public void PlayPhotoSfx()
    {
        if (string.IsNullOrEmpty(photoSfxId)) return;

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySpatialSound(photoSfxId, transform.position, photoSfxVolume);
        }
    }
}
