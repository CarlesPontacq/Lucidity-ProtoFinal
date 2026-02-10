using NUnit.Framework;
using UnityEngine;

public class CameraObject : ObjectInteraction
{
    public override void Interact()
    {
        GameManager.Instance.CameraGrabbed();
        Destroy(gameObject);
    }
}
