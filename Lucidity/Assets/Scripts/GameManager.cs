using System;
using System.Collections;
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

    public void PlayerDied()
    {
        if (isDying) return;

        if (PlayerRef == null)
            CachePlayerRoot();

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        isDying = true;

        SetPlayerControlEnabled(false);

        if (deathEffect != null)
            yield return deathEffect.PlayDeathSequence();
        else
            yield return new WaitForSecondsRealtime(5f);

        ResetLoops();
        loopManager?.StartNextLoop();

        TeleportPlayerToStart_RigidbodySafe();

        if (deathEffect != null)
        {
            deathEffect.ClearOverlays();
            deathEffect.RestoreAfterRespawn();
        }

        SetPlayerControlEnabled(true);

        isDying = false;
    }

    private void SetPlayerControlEnabled(bool enabled)
    {
        if (disableOnDeath == null) return;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            if (disableOnDeath[i] != null)
                disableOnDeath[i].enabled = enabled;
        }
    }

    private void TeleportPlayerToStart_RigidbodySafe()
    {
        if (PlayerRef == null) return;

        GameObject sp = GameObject.FindGameObjectWithTag(playerSpawnTag);
        if (sp == null)
        {
            Debug.LogWarning($"No hay objeto con tag {playerSpawnTag} en la escena.");
            return;
        }

        Transform spawn = sp.transform;

        var cols = PlayerRef.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].enabled = true;
        }

        Rigidbody rb = PlayerRef.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.detectCollisions = true;
            rb.isKinematic = false;

            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

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
