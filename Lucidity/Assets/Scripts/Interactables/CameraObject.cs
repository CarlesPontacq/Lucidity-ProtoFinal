using NUnit.Framework;
using UnityEngine;

public class CameraObject : ObjectInteraction
{
    [SerializeField] ItemData itemData;

    public override void Interact()
    {
        GameManager.Instance.CameraGrabbed(itemData);
        Destroy(gameObject);
    }
}
