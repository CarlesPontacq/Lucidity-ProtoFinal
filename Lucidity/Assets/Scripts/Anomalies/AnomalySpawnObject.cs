using UnityEngine;

public class AnomalySpawnObject : Anomaly
{
    [SerializeField] private GameObject objectToShow; 

    protected override void OnActivate()
    {
        if (objectToShow) objectToShow.SetActive(true);
    }

    protected override void OnDeactivate()
    {
        if (objectToShow) objectToShow.SetActive(false);
    }
}
