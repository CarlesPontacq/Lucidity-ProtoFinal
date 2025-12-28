using UnityEngine;

public class AnomalySwapObjects : Anomaly
{
    [SerializeField] private GameObject normalRoot;   
    [SerializeField] private GameObject anomalyRoot;  

    protected override void OnActivate()
    {
        base.OnActivate();
        if (normalRoot) normalRoot.SetActive(false);
        if (anomalyRoot) anomalyRoot.SetActive(true);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        if (normalRoot) normalRoot.SetActive(true);
        if (anomalyRoot) anomalyRoot.SetActive(false);
    }
}
