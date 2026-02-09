using NUnit.Framework;
using UnityEngine;

public class CameraObject : ObjectInteraction
{
    public override void Interact()
    {
        Debug.Log("Interactuar camara");
        GameManager.Instance.CameraGrabbed();
        Destroy(gameObject);
    }
}
