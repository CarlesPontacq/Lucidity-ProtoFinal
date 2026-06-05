using UnityEngine;

public class PillsObject : ObjectInteraction
{
    public override void Interact()
    {
        GameManager.GrabHandlerRef.PillsGrabbed();
    }
}
