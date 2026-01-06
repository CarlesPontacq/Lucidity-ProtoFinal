using UnityEngine;

public class LoopManager : MonoBehaviour
{
    [SerializeField] private AnomalyManager anomalyManager;

    private int loopIndex = 0;

    private void Start()
    {
        StartNextLoop();
    }

    public void StartNextLoop()
    {
        loopIndex++;
        anomalyManager.StartNewLoop();

        Debug.Log($"Loop {loopIndex}: entries = {anomalyManager.EntryCount}, spawned = {anomalyManager.ActiveSpawnedCount}");
    }
}
