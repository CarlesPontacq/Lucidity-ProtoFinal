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
        StartBaseLoop();
    }

    public void StartNextLoop()
    {
        GameManager.Instance.CameraGrabbed();
        GameManager.Instance.ReportSheetGrabbed();

        if (Time.unscaledTime < nextAllowedTime)
            return;

        nextAllowedTime = Time.unscaledTime + nextLoopCooldown;

        if (GameManager.Instance.GetCurrentLoopIndex() == 0)
        {
            Debug.Log("Loop 0 -> se avanza directamente");
            GameManager.Instance.AddLoopToCount();
            StartLoopFresh();
        }

        if (reportState != null && reportState.HasSubmittedReport)
        {
            if (reportState.WasCorrect)
            {
                Debug.Log("Report correcto -> sumo loop");
                GameManager.Instance.AddLoopToCount();
                StartLoopFresh();
            }
            else
            {
                Debug.Log("Report incorrecto -> reseteo loops");
                GameManager.Instance.ResetLoops();
                StartBaseLoop();
            }
        }
        else
        {
            Debug.Log("Sin reporte enviado (primer loop o no firmó) -> no toco el contador");
        }

    }

    private void StartBaseLoop()
    {
        if (reportState != null)
            reportState.ResetForNewLoop();

        if (interactableDoors != null)
        {
            for (int i = 0; i < interactableDoors.Count; i++)
            {
                if (interactableDoors[i] != null)
                    interactableDoors[i].CloseDoor(false);
            }
        }

        if (documentationMode != null)
            documentationMode.ResetReels();

        if (exitLamp != null)
            exitLamp.SetCanPass(true);

        if (zonesManager != null)
            zonesManager.UpdateZoneDoors(GameManager.Instance.GetCurrentLoopIndex());

        if (anomalyManager != null)
            anomalyManager.ClearSpawned();

        if (exitDoor != null)
        {
            exitDoor.Unlock();        }

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

        if (anomalyManager != null)
            anomalyManager.StartNewLoop();
        else
            Debug.LogWarning("LoopManager: anomalyManager es null (no puedo spawnear anomalías).");

        if (enemySpawner != null)
            enemySpawner.SpawnForNewLoop();

    }
}
