using UnityEngine;

public class LoopManager : MonoBehaviour
{
    [SerializeField] private AnomalyManager anomalyManager;
    [SerializeField] private ReportResultState reportState;
    [SerializeField] private DoorInteraction exitDoor;

    private int loopIndex = 0;

    private void Start()
    {
        StartNextLoop();
    }

    public void StartNextLoop()
    {
        loopIndex++;

        // 1) Reset del resultado del documento
        if (reportState != null)
            reportState.ResetForNewLoop();

        // 2) Bloquear la puerta de salida otra vez
        if (exitDoor != null)
            exitDoor.LockExitDoor();

        // 3) Spawnear anomalías del loop
        if (anomalyManager != null)
            anomalyManager.StartNewLoop();

        Debug.Log($"Loop {loopIndex} started");
    }
}
