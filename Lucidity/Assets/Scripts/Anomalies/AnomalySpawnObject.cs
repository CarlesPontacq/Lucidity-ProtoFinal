using UnityEngine;

public class AnomalySpawnObject : Anomaly
{
    [SerializeField] private GameObject objectToShow; 

    protected override void OnActivate()
    {
        base.OnActivate();
        if (objectToShow) objectToShow.SetActive(true);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        if (objectToShow) objectToShow.SetActive(false);
    }

    private void Update()
    {
        if (IsSpawnedThisLoop)
            objectToShow.SetActive(true);
        else
            objectToShow.SetActive(false);
    }
}
