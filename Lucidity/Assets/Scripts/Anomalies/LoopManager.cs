using System;
using System.Collections.Generic;
using UnityEngine;

public class LoopManager : MonoBehaviour
{
    public static event Action<int> OnLoopStarted;
    [SerializeField] private AnomalyManager anomalyManager;
    [SerializeField] private ReportSheetOverlayUI reportSheetOverlayScript;
    [SerializeField] private ReportResultState reportState;
    [SerializeField] private DoorInteraction exitDoor;
    [SerializeField] private List<ObjectInteraction> interactableObjects;
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
        StartNextLoop();
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
            }
            else
            {
                Debug.Log("Report incorrecto -> reseteo loops");
                GameManager.Instance.SubtractLoopToCount();
            }
            
            StartLoopFresh();
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

        if (interactableObjects != null)
        {
            for (int i = 0; i < interactableObjects.Count; i++)
            {
                if (interactableObjects[i] != null)
                    interactableObjects[i].ResetState();
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

        if (reportSheetOverlayScript != null)
            reportSheetOverlayScript.ResetDocumentState();

        if (exitDoor != null)
            exitDoor.LockExitDoor();

        if (interactableObjects != null)
        {
            for (int i = 0; i < interactableObjects.Count; i++)
            {
                if (interactableObjects[i] != null)
                    interactableObjects[i].ResetState();
            }
        }

        if (exitBlocker != null)
            exitBlocker.LockPassage();

        if (cameraFunctionality != null)
            cameraFunctionality.ResetReels();

        if (exitLamp != null)
            exitLamp.TurnOn();

        if (enemySpawner != null)
            enemySpawner.ClearEnemy();

        if (anomalyManager != null)
            anomalyManager.StartNewLoop();
        else
            Debug.LogWarning("LoopManager: anomalyManager es null (no puedo spawnear anomal�as).");

        OnLoopStarted?.Invoke(GameManager.Instance.GetCurrentLoopIndex());
    }
}
