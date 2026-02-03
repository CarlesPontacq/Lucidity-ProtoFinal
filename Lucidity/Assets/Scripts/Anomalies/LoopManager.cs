using UnityEngine;
using System.Collections.Generic;

public class LoopManager : MonoBehaviour
{
    [SerializeField] private ZonesManager zonesManager;
    [SerializeField] private AnomalyManager anomalyManager;
    [SerializeField] private ReportResultState reportState;
    [SerializeField] private DoorInteraction exitDoor;
    [SerializeField] private List<DoorInteraction> interactableDoors;
    [SerializeField] private DocumentationMode documentationMode;

    [Header("Optional")]
    [SerializeField] private ExitDoorBlocker exitBlocker;
    [SerializeField] private ExitLightEmissionMapSwitcher exitLamp;

    [Header("Safety")]
    [Tooltip("Evita avanzar múltiples loops por doble trigger.")]
    [SerializeField] private float nextLoopCooldown = 0.25f;

    [SerializeField] private EnemySpawner enemySpawner;


    private float nextAllowedTime = 0f;

    private void Start()
    {
        StartLoopFresh();
    }

    public void StartNextLoop()
    {
        if (Time.unscaledTime < nextAllowedTime)
            return;

        nextAllowedTime = Time.unscaledTime + nextLoopCooldown;

        if (reportState != null && reportState.HasSubmittedReport)
        {
            if (reportState.WasCorrect)
            {
                Debug.Log("Report correcto -> sumo loop");
                GameManager.Instance.AddLoopToCount();
            }
            else
            {
                Debug.Log("Report incorrecto -> reseteo loops");
                GameManager.Instance.ResetLoops();
            }
        }
        else
        {
            Debug.Log("Sin reporte enviado (primer loop o no firmó) -> no toco el contador");
        }

        StartLoopFresh();
    }

    private void StartLoopFresh()
    {
        if (reportState != null)
            reportState.ResetForNewLoop();

        if (exitDoor != null)
            exitDoor.LockExitDoor();

        if (interactableDoors != null)
        {
            for (int i = 0; i < interactableDoors.Count; i++)
            {
                if (interactableDoors[i] != null)
                    interactableDoors[i].CloseDoor(false);
            }
        }

        if (exitBlocker != null)
            exitBlocker.LockPassage();

        if (documentationMode != null)
            documentationMode.ResetReels();

        if (exitLamp != null)
            exitLamp.SetCanPass(false);

        if (zonesManager != null)
            zonesManager.UpdateZoneDoors(GameManager.Instance.GetCurrentLoopIndex());

        Debug.Log("Test");

        if (anomalyManager != null)
            anomalyManager.StartNewLoop();
        else
            Debug.LogWarning("LoopManager: anomalyManager es null (no puedo spawnear anomalías).");

        if (enemySpawner != null)
            enemySpawner.SpawnForNewLoop();

    }
}
