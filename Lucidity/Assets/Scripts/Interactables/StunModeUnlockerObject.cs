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
        GameManager.Instance.StunModeUnlockerGrabbed(itemData);
        Destroy(gameObject);
    }
}
