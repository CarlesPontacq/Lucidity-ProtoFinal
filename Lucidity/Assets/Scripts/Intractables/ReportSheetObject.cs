using NUnit.Framework;
using UnityEngine;

public class ReportSheetObject : ObjectInteraction
{
    public override void Interact()
    {
        Debug.Log("Interactuar papel");
        GameManager.Instance.ReportSheetGrabbed();
        Destroy(gameObject);
    }
}
