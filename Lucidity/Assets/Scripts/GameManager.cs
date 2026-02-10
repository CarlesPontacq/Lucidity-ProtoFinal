using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static GameObject PlayerRef { get; private set; }

    [Header("References")]
    [SerializeField] private LoopCounter loopCounterUI;
    [SerializeField] private LoopManager loopManager;

    [Header("Death FX")]
    [SerializeField] private DeathCameraEffect deathEffect;

    [Header("Disable while dead (NO metas Rigidbody/Colliders aquí)")]
    [SerializeField] private MonoBehaviour[] disableOnDeath;

    [Header("Spawn")]
    [SerializeField] private string playerSpawnTag = "PlayerSpawn";

    private int currentLoop = 0;
    private bool isDying = false;

    [SerializeField] CameraManager cameraManager;
    [SerializeField] DocumentationMode documentationMode;
    [SerializeField] ReportSheetOverlayUI reportSheet;
    private bool cameraGrabbed = false; 
    private bool reportSheetGrabbed = false; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CachePlayerRoot();
    }

    private void CachePlayerRoot()
    {
        var pm = FindAnyObjectByType<PlayerMovement>();
        if (pm != null)
        {
            PlayerRef = pm.gameObject;

            if (deathEffect == null)
                deathEffect = PlayerRef.GetComponentInChildren<DeathCameraEffect>(true);

            Debug.Log($"[GM] PlayerRef = {PlayerRef.name}");
        }
        else
        {
            Debug.LogWarning("[GM] No encontré PlayerMovement. PlayerRef no asignado.");
        }
    }

    // ====================
    // LOOP COUNTER
    // ====================
    public int GetCurrentLoopIndex() => currentLoop;

    public void AddLoopToCount()
    {
        currentLoop++;
        loopCounterUI.SetLoopCounterText(currentLoop);
    }

    public void ResetLoops()
    {
        currentLoop = 0;
        loopCounterUI.SetLoopCounterText(currentLoop);
    }

    public void OnExitDoorCrossed()
    {
        loopManager.StartNextLoop();
    }

    public void CameraGrabbed()
    {
        cameraGrabbed = true;
        documentationMode.isUnlocked = true;
        cameraManager.SetStartingCameraMode();
    }

    public void ReportSheetGrabbed()
    {
        reportSheetGrabbed = true;
        reportSheet.Grab();
    }

    public bool GetCameraGrabbed()
    {
        return cameraGrabbed;
    }

    public bool GetReportSheetGrabbed()
    {
        return reportSheetGrabbed;
    }

}
