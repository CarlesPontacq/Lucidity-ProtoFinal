using System;
using UnityEngine;

public class CameraEventBroadcaster : MonoBehaviour
{
    [SerializeField] private AnomalyManager manager;

    void Start()
    {
        if (manager == null)
            manager = GameObject.FindAnyObjectByType<AnomalyManager>();
    }

    public void NotifyModeActivated()
    {
        manager = FindAnyObjectByType<AnomalyManager>();
        if (manager == null) return;

        foreach (var anomaly in manager.GetSpawnedEnemiesThisLoop())
            anomaly.OnCameraModeActivated();
    }

    public void NotifyModeDeactivated()
    {
        manager = FindAnyObjectByType<AnomalyManager>();
        if (manager == null) return;

        foreach (var anomaly in manager.GetSpawnedEnemiesThisLoop())
            anomaly.OnCameraModeDeactivated();
    }
}
