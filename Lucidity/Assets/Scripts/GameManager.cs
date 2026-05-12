using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static GameObject PlayerRef { get; private set; }

    public event Action OnCameraTaken;
    public event Action OnReportSheetTaken;


    [Header("Player settings")]
    [SerializeField] private bool toggleSprint;

    [Header("References")]
    [SerializeField] private LoopCounter loopCounterUI;
    [SerializeField] private LoopManager loopManager;

    [Header("Death FX")]
    [SerializeField] private DeathCameraEffect deathEffect;

    [Header("Scene Transition")]
    [SerializeField] private TransitionToAct3 sceneTransition;

    [Header("Disable while dead (NO metas Rigidbody/Colliders aquí)")]
    [SerializeField] private MonoBehaviour[] disableOnDeath;

    [Header("Player Visuals")]
    [SerializeField] private GameObject playerBody;

    [Header("Spawn")]
    [SerializeField] private string playerSpawnTag = "PlayerSpawn";

    [Header("Pickups / Systems")]
    public bool startWithCamera = false;
    public bool startWithReportSheet = false;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private CameraFunctionality cameraFunctionality;
    [SerializeField] private ReportSheetOverlayUI reportSheet;
    [SerializeField] private ItemInfoOverlay itemInfoOverlay;
    [SerializeField] private PlayerArmsAnimationController handsWithCamera;

    [Header("Death Safety")]
    [SerializeField] private float deathCooldown = 0.35f;

    [Header("Physics Safety")]
    [Tooltip("Layer que debe tener el Player root tras respawn (opcional). Déjalo en -1 para no tocar layer.")]
    [SerializeField] private int forcePlayerLayer = -1;

    [Header("Exit Loop")]
    [SerializeField] private int lastLoop = 8;

    [SerializeField] private CameraRotation cameraRotation;

    private int currentLoop = 0;
    private int minLoop = 1;
    public bool isDying = false;
    private float nextAllowedDeathTime = 0f;

    private bool cameraGrabbed = false;
    private bool stunModeUnlockerGrabbed = false;
    private bool reportSheetGrabbed = false;
    private bool gunGrabbed = false;
    public bool finishedLoops = false;

    public event Action OnStunUnlocked;

    private void Awake()
    {
        if(PlayerRef == null)
        {
            PlayerRef = GameObject.FindGameObjectWithTag("Player");
        }

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

    private void Start()
    {
        CachePlayerRoot();
        cameraRotation.SetControlEnabled(true);
        SetPlayerControlEnabled(true);
        finishedLoops = false;

        SetHandsWithCameraVisibility(false);

        if (startWithCamera)
        {
            cameraGrabbed = true;

            if (cameraFunctionality != null)
                cameraFunctionality.isUnlocked = true;

            if (cameraManager != null)
                cameraManager.SetFunctionality(cameraFunctionality);

            SetHandsWithCameraVisibility(true);
        }

        if (startWithReportSheet)
        {
            reportSheetGrabbed = true;
            reportSheet.Grab();
        }
    }

    private void SetUpCharacterOnNewScene()
    {
        cameraRotation.SetControlEnabled(true);
        SetPlayerControlEnabled(true);
        finishedLoops = false;
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

    #region Pickups

    public void CameraGrabbed(ItemData itemData)
    {
        cameraGrabbed = true;

        if (cameraFunctionality != null)
            cameraFunctionality.isUnlocked = true;

        if (itemInfoOverlay != null)
            itemInfoOverlay.OpenInfo(itemData);

        if (cameraManager != null)
            cameraManager.SetFunctionality(cameraFunctionality);

        SetHandsWithCameraVisibility(true);

        OnCameraTaken?.Invoke();
    }

    public void ReportSheetGrabbed(ItemData itemData)
    {
        reportSheetGrabbed = true;

        if (reportSheet != null)
            reportSheet.Grab();

        if (itemInfoOverlay != null)
            itemInfoOverlay.OpenInfo(itemData);

        OnReportSheetTaken?.Invoke();
    }

    public void GunGrabbed()
    {
        gunGrabbed = true;
        finishedLoops = true;

        SetPlayerControlEnabled(false);
        cameraRotation.SetControlEnabled(false);

        StartCoroutine(sceneTransition.PlayTransition());
    }

    public void StunModeUnlockerGrabbed(ItemData itemData)
    {
        stunModeUnlockerGrabbed = true;

        if (cameraManager != null)
            cameraManager.OnGrabbedFlash();

        if (itemInfoOverlay != null)
            itemInfoOverlay.OpenInfo(itemData);

        OnStunUnlocked?.Invoke();
    }

    public bool HasUnlockedStun() => stunModeUnlockerGrabbed;

    public void SetHandsWithCameraVisibility(bool visibility)
    {
        if (handsWithCamera == null) return;

        if (visibility)
            handsWithCamera.ShowArms();
        else
            handsWithCamera.HideArms();
    }

    #endregion

    // ===============================
    // DEATH SYSTEM
    // ===============================

    public void PlayerDied()
    {
        if (CheatsManager.Instance != null && CheatsManager.Instance.currentlyImmortal) return;
        if (isDying) return;
        if (Time.time < nextAllowedDeathTime) return;

        nextAllowedDeathTime = Time.time + deathCooldown;

        if (PlayerRef == null)
            CachePlayerRoot();

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        isDying = true;

        Time.timeScale = 1f;

        SetPlayerControlEnabled(false);

        // Ocultar body
        SetPlayerBodyVisible(false);
        cameraRotation.SetControlEnabled(false);

        // Animación de muerte
        if (deathEffect != null)
            yield return deathEffect.PlayDeathSequence();
        else
            yield return new WaitForSecondsRealtime(5f);

        // Reset loops
        SubtractLoopToCount();
        if (loopManager != null)
            loopManager.StartLoopFresh();

        yield return TeleportAndRearmPhysics();

        if (deathEffect != null)
        {
            deathEffect.ClearOverlays();
            deathEffect.RestoreAfterRespawn();
        }

        SetPlayerBodyVisible(true);
        cameraRotation.SetControlEnabled(true);
        cameraRotation.ResetOrientation();

        SetPlayerControlEnabled(true);

        var deathTouch = PlayerRef.GetComponentInChildren<PlayerDeathOnEnemyTouch>(true);
        if (deathTouch != null)
            deathTouch.ResetDeathTrigger();

        isDying = false;
    }

    public void SetPlayerControlEnabled(bool enabled)
    {
        if (disableOnDeath == null) return;

        for (int i = 0; i < disableOnDeath.Length; i++)
            if (disableOnDeath[i] != null)
                disableOnDeath[i].enabled = enabled;
    }

    private void SetPlayerBodyVisible(bool visible)
    {
        if (playerBody == null) return;

        Renderer[] renderers = playerBody.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].enabled = visible;
    }

    private IEnumerator TeleportAndRearmPhysics()
    {
        if (PlayerRef == null) yield break;

        GameObject sp = GameObject.FindGameObjectWithTag(playerSpawnTag);
        if (sp == null)
        {
            Debug.LogWarning($"No hay objeto con tag {playerSpawnTag}.");
            yield break;
        }

        Transform spawn = sp.transform;

        var cols = PlayerRef.GetComponentsInChildren<Collider>(true);
        foreach (var col in cols)
            col.enabled = true;

        Rigidbody rb = PlayerRef.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.position = spawn.position;
            rb.rotation = spawn.rotation;

            rb.WakeUp();
        }
        else
        {
            PlayerRef.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
        }

        Physics.SyncTransforms();

        yield return null;
    }

    public bool GetCameraGrabbed() => cameraGrabbed;

    public int GetCurrentLoop() => currentLoop;
    public bool GetReportSheetGrabbed() => reportSheetGrabbed;
    public bool GetToggleSprint() => toggleSprint;
    public void SetToggleSprint(bool sprintToggle) => toggleSprint = sprintToggle;
}
