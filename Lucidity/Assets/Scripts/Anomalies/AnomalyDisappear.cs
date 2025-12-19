using UnityEngine;

public class AnomalyDisappear : Anomaly
{
    [SerializeField] private GameObject targetToHide;

    protected override void OnActivate()
    {
        if (targetToHide) targetToHide.SetActive(false);
    }

    protected override void OnDeactivate()
    {
        if (targetToHide) targetToHide.SetActive(true);
    }
}

