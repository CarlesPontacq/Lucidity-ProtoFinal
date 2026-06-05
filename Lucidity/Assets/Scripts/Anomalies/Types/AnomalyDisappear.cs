using UnityEngine;

public class AnomalyDisappear : Anomaly
{
    [SerializeField] private GameObject targetToHide;

    protected override void OnActivate()
    {
        base.OnActivate();
        if (targetToHide) targetToHide.SetActive(false);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        if (targetToHide) targetToHide.SetActive(true);
    }
}

