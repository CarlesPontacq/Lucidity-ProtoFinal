using System;
using System.Collections.Generic;
using UnityEngine;

public class LoopManager : MonoBehaviour
{
    public static event Action<int> OnLoopStarted;
    [SerializeField] private AnomalyManager anomalyManager;
    [SerializeField] private ReportResultState reportState;
    [SerializeField] private DoorInteraction exitDoor;
    [SerializeField] private List<DoorInteraction> interactableDoors;
    [SerializeField] private CameraFunctionality cameraFunctionality;

    [Header("Optional")]
    [SerializeField] private ExitDoorBlocker exitBlocker;
    [SerializeField] private ExitLamp exitLamp;

    [Header("Safety")]
    [Tooltip("Evita avanzar multiples loops por doble trigger.")]
    [SerializeField] private float nextLoopCooldown = 0.25f;

    [SerializeField] private EnemyLoopSpawner enemySpawner;

    private float nextAllowedTime = 0f;

    private void Start()
    {
        StartBaseLoop();
    }

    public void StartNextLoop()
    {
        if (Time.unscaledTime < nextAllowedTime)
            return;

        nextAllowedTime = Time.unscaledTime + nextLoopCooldown;

        if(GameManager.Instance.GetCurrentLoopIndex() == 0)
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
                GameManager.Instance.SubtractLoopToCount();
                StartLoopFresh();
            }
        }
        else
        {
            Debug.Log("Sin reporte enviado (primer loop o no firm�) -> no toco el contador");
        }

    }

    public void StartBaseLoop()
    {
        if (reportState != null)
            reportState.ResetForNewLoop();

        if (interactableDoors != null)
        {
            for (int i = 0; i < interactableDoors.Count; i++)
            {
                if (interactableDoors[i] != null)
                    interactableDoors[i].ResetToInitialState(false);
            }
        }

        if (cameraFunctionality != null)
            cameraFunctionality.ResetReels();

        if (exitLamp != null)
            exitLamp.TurnOff();

        if (anomalyManager != null)
            anomalyManager.ClearSpawned();

        if(enemySpawner != null)
            enemySpawner.ResetCurrentLoopIndex();

        if (exitDoor != null)
        {
            exitDoor.Unlock();        
        }
    }

    public void StartLoopFresh()
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
                    interactableDoors[i].ResetToInitialState(false);
            }
        }

        if (exitBlocker != null)
            exitBlocker.LockPassage();

        if (cameraFunctionality != null)
            cameraFunctionality.ResetReels();

        if (exitLamp != null)
            exitLamp.TurnOn();

        if (anomalyManager != null)
            anomalyManager.StartNewLoop();
        else
            Debug.LogWarning("LoopManager: anomalyManager es null (no puedo spawnear anomal�as).");

        OnLoopStarted?.Invoke(GameManager.Instance.GetCurrentLoopIndex());
    }
}
