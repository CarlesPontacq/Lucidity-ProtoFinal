using NUnit.Framework;
using UnityEngine;

public class StunModeUnlockerObject : ObjectInteraction
{
    [SerializeField] ItemData itemData;
    [SerializeField] private string grabFlashSFX = "GrabFlash";
    private float grabFlashVolumeSFX = 1f;
    public override void Interact()
    {
        SFXManager.Instance.PlayGlobalSound(grabFlashSFX, grabFlashVolumeSFX);
        GameManager.GrabHandlerRef.StunModeUnlockerGrabbed(itemData);
        Destroy(gameObject);
    }
}
