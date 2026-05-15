using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static GameObject PlayerRef { get; private set; }
    public static GrabHandler GrabHandlerRef { get; private set; }
    public static CinematicHandler CinematicHandlerRef { get; private set; }

    [Header("Player settings")]
    [SerializeField] private bool toggleSprint;

    [Header("References")]
    [SerializeField] private LoopCounter loopCounterUI;
    [SerializeField] private LoopManager loopManager;

    [Header("Exit Loop")]
    [SerializeField] private int lastLoop = 8;

    [SerializeField] private CameraRotation cameraRotation;

    private int currentLoop = 0;
    private int minLoop = 1;

    public bool finishedLoops = false;


    private void Awake()
    {
        SetUpReferences();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            SetUpCharacterOnNewScene();
            Destroy(gameObject);
        }

    }

    private void SetUpReferences()
    {
        CachePlayerRoot();

        if(GrabHandlerRef  == null)
        {
            GrabHandlerRef = FindAnyObjectByType<GrabHandler>();
        }

        if (CinematicHandlerRef == null)
        {
            CinematicHandlerRef = FindAnyObjectByType<CinematicHandler>();
        }
    }

    private void Start()
    {
        CachePlayerRoot();
        cameraRotation.SetControlEnabled(true);
        CinematicHandlerRef.SetPlayerControlEnabled(true);
        finishedLoops = false;
    }

    private void SetUpCharacterOnNewScene()
    {
        cameraRotation.SetControlEnabled(true);
        CinematicHandlerRef.SetPlayerControlEnabled(true);
        finishedLoops = false;
    }

    private void CachePlayerRoot()
    {
        var pm = FindAnyObjectByType<PlayerMovement>();
        if (pm != null)
        {
            PlayerRef = pm.gameObject;

            Debug.Log($"[GM] PlayerRef = {PlayerRef.name}");
        }
        else
        {
            Debug.LogWarning("[GM] No encontré PlayerMovement. PlayerRef no asignado.");
        }
    }

    #region Loops

    public int GetCurrentLoopIndex() => currentLoop;
    public void SetCurrentLoopIndex(int newIndex) => currentLoop = newIndex;

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
        if(currentLoop <= minLoop) currentLoop = minLoop;

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
        if (loopManager != null)
            loopManager.StartNextLoop();
    }

    #endregion



    public void SetFinishedLoops(bool finished) => finishedLoops = finished;
    public bool GetToggleSprint() => toggleSprint;
    public void SetToggleSprint(bool sprintToggle) => toggleSprint = sprintToggle;
}
