using NUnit.Framework;
using UnityEngine;

public class StunModeUnlockerObject : ObjectInteraction
{
    [SerializeField] ItemData itemData;

    public override void Interact()
    {
        GameManager.Instance.StunModeUnlockerGrabbed(itemData);
        Destroy(gameObject);
    }
}
