using System;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class LoopManager : MonoBehaviour
{
    public static event Action<int> OnLoopStarted;

    [Header("References")]
    [SerializeField] private AnomalyManager anomalyManager;
    [SerializeField] private ReportSheetOverlayUI reportSheetOverlayScript;
    [SerializeField] private ReportResultState reportState;
    [SerializeField] private DoorController exitDoor;
    [SerializeField] private List<ObjectInteraction> interactableObjects;
    [SerializeField] private CameraFunctionality cameraFunctionality;

    [Header("Loop Config")]
    [SerializeField] private LoopCounter loopCounterUI;
    [SerializeField] private int lastLoop = 4;
    private int currentLoop = 1;
    private int minLoop = 1;
    private bool finishedLoops = false;

    [Header("Optional")]
    [SerializeField] private ExitDoorBlocker exitBlocker;
    [SerializeField] private ExitLamp exitLamp;

    [Header("Safety")]
    [Tooltip("Evita avanzar multiples loops por doble trigger.")]
    [SerializeField] private float nextLoopCooldown = 0.25f;

    [SerializeField] private EnemyLoopSpawner enemySpawner;

    private float nextAllowedTime = 0f;
    private bool firstLoop = true;

    private void Start()
    {
        StartNextLoop();
    }

    #region Loop Functionality
    public void StartNextLoop()
    {
        if (Time.unscaledTime < nextAllowedTime)
            return;

        nextAllowedTime = Time.unscaledTime + nextLoopCooldown;

        if(firstLoop)
        {
            StartLoopFresh();
            firstLoop = false;
        }

        if (reportState != null && reportState.HasSubmittedReport)
        {
            if (reportState.WasCorrect)
            {
                Debug.Log("Report correcto -> sumo loop");
                AddLoopToCount();
            }
            else
            {
                Debug.Log("Report incorrecto -> reseteo loops");
                SubtractLoopToCount();
            }
            
            StartLoopFresh();
        }
        else
        {
            Debug.Log("Sin reporte enviado (primer loop o no firm�) -> no toco el contador");
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
        {
            enemySpawner.StopAllCoroutines();
            enemySpawner.ClearEnemy();
        }

        if (anomalyManager != null)
            anomalyManager.StartNewLoop();
        else
            Debug.LogWarning("LoopManager: anomalyManager es null (no puedo spawnear anomal�as).");

        OnLoopStarted?.Invoke(GetCurrentLoopIndex());
    }
    #endregion

    #region Loop Control
    public void AddLoopToCount()
    {
        currentLoop++;
        if (loopCounterUI != null) loopCounterUI.SetLoopCounterText(currentLoop);
    }

    public void HasFinishedLastLoop()
    {
        if (currentLoop >= lastLoop) finishedLoops = true;
    }

    public void SubtractLoopToCount()
    {
        currentLoop--;
        if (currentLoop <= minLoop) currentLoop = minLoop;

        if (loopCounterUI != null) loopCounterUI.SetLoopCounterText(currentLoop);
    }

    public void ResetLoops()
    {
        currentLoop = 0;
        if (loopCounterUI != null)
            loopCounterUI.SetLoopCounterText(currentLoop);
    }

    public void OnExitDoorCrossed()
    {
        StartNextLoop();
    }

    public int GetCurrentLoopIndex() => currentLoop;
    public bool GetFinishedLoops() => finishedLoops;
    public void SetCurrentLoopIndex(int newIndex) => currentLoop = newIndex;
    public void SetFinishedLoops(bool finished) => finishedLoops = finished;
    #endregion
}
