using NUnit.Framework;
using UnityEngine;

public class ReportSheetObject : ObjectInteraction
{
    [SerializeField] ItemData itemData;
    [SerializeField] private string grabReportSFX = "";
    private float grabReportVolumeSFX = 1f;

    public override void Interact()
    {
        SFXManager.Instance.PlayGlobalSound(grabReportSFX, grabReportVolumeSFX);
        GameManager.Instance.ReportSheetGrabbed(itemData);
        Destroy(gameObject);
    }
}
