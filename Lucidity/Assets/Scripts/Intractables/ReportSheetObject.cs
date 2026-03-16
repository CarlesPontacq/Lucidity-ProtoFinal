using NUnit.Framework;
using UnityEngine;

public class ReportSheetObject : ObjectInteraction
{
    [SerializeField] ItemData itemData;

    public override void Interact()
    {
        GameManager.Instance.ReportSheetGrabbed(itemData);
        Destroy(gameObject);
    }
}
