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

    public void NotifyModeActivated(CameraMode mode)
    {
        manager = FindAnyObjectByType<AnomalyManager>();
        if (manager == null) return;

        foreach (var anomaly in manager.GetSpawnedEnemiesThisLoop())
            anomaly.OnCameraModeActivated(mode);
    }

    public void NotifyModeDeactivated(CameraMode mode)
    {
        manager = FindAnyObjectByType<AnomalyManager>();
        if (manager == null) return;

        foreach (var anomaly in manager.GetSpawnedEnemiesThisLoop())
            anomaly.OnCameraModeDeactivated(mode);
    }
}
