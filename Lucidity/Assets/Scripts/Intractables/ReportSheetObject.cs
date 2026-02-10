using NUnit.Framework;
using UnityEngine;

public class ReportSheetObject : ObjectInteraction
{
    public override void Interact()
    {
        GameManager.Instance.ReportSheetGrabbed();
        Destroy(gameObject);
    }
}
