using UnityEngine;

public class GunObject : ObjectInteraction
{
    public override void Interact()
    {
        GameManager.GrabHandlerRef.GunGrabbed();
        Destroy(gameObject);
    }
}
