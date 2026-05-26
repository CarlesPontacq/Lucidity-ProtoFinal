using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static GameObject PlayerRef { get; private set; }
    public static GrabHandler GrabHandlerRef { get; private set; }
    public static DeathHandler DeathHandlerRef { get; private set; }
    public static LoopManager LoopManagerRef { get; private set; }
    public static PlayerInput PlayerInput { get; private set; }

    [SerializeField] private CameraRotation cameraRotation;

    [Header("Player settings")]
    [SerializeField] private bool toggleSprint;

    [Header("Disable components")]
    [SerializeField] private MonoBehaviour[] scriptsToDisble;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            SetUpCharacterOnNewScene();
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        PlayerInput = FindFirstObjectByType<PlayerInput>();
        PlayerInput[] inputs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);

        SetUpReferences();
    }

    private void SetUpReferences()
    {
        CachePlayerRoot();

        if(GrabHandlerRef  == null)
        {
            GrabHandlerRef = FindAnyObjectByType<GrabHandler>();
        }

        if (DeathHandlerRef == null)
        {
            DeathHandlerRef = FindAnyObjectByType<DeathHandler>();
        }

        if (LoopManagerRef == null)
        {
            LoopManagerRef = FindAnyObjectByType<LoopManager>();
        }
    }

    private void Start()
    {
        CachePlayerRoot();
        cameraRotation.SetControlEnabled(true);
        SetPlayerControlEnabled(true);
        
        if(LoopManagerRef != null)
            LoopManagerRef.SetFinishedLoops(false);
    }

    private void SetUpCharacterOnNewScene()
    {
        cameraRotation.SetControlEnabled(true);
        SetPlayerControlEnabled(true);

        if (LoopManagerRef != null)
            LoopManagerRef.SetFinishedLoops(false);
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

    public void SetPlayerControlEnabled(bool enabled)
    {
        if (scriptsToDisble == null) return;

        for (int i = 0; i < scriptsToDisble.Length; i++)
            if (scriptsToDisble[i] != null)
                scriptsToDisble[i].enabled = enabled;
    }

    public bool GetToggleSprint() => toggleSprint;
    public void SetToggleSprint(bool sprintToggle) => toggleSprint = sprintToggle;
}
