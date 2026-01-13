using UnityEngine;
using System.Collections.Generic;

public class LoopManager : MonoBehaviour
{
    [SerializeField] private AnomalyManager anomalyManager;
    [SerializeField] private ReportResultState reportState;
    [SerializeField] private DoorInteraction exitDoor;
    [SerializeField] private List<DoorInteraction> interactableDoors;

    [Header("Optional")]
    [SerializeField] private ExitDoorBlocker exitBlocker;
    [SerializeField] private ExitLightEmissionMapSwitcher exitLamp;

    [Header("Safety")]
    [Tooltip("Evita avanzar múltiples loops por doble trigger.")]
    [SerializeField] private float nextLoopCooldown = 0.25f;

    private int loopIndex = 0;
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
        loopIndex++;

        if (reportState != null)
            reportState.ResetForNewLoop();

        if (exitDoor != null)
            exitDoor.LockExitDoor();

        for (int i = 0; i < interactableDoors.Count; i++)
            interactableDoors[i].CloseDoor(false);

        if (exitBlocker != null)
            exitBlocker.LockPassage();

        if (exitLamp != null)
            exitLamp.SetCanPass(false);
        else
            Debug.LogWarning("LoopManager: exitLamp NO asignada (no puedo poner la luz en rojo).");

        if (anomalyManager != null)
            anomalyManager.StartNewLoop();

        Debug.Log($"Loop {loopIndex} started");
    }

    private void Update()
    {
        if (GameManager.Instance.GetNewLoop())
        {
            StartNextLoop();
            GameManager.Instance.SetNewLoop(false);
        }
    }
}
