using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static GameObject PlayerRef { get; private set; }

    [SerializeField] private LoopCounter loopCounterUI;
    [SerializeField] private LoopManager loopManager;
    private int currentLoop = 0;

    [Header("Death Sequence")]
    [SerializeField] private DeathCameraEffect deathEffect;        
    [SerializeField] private MonoBehaviour[] disableOnDeath;       
    [SerializeField] private string playerSpawnTag = "PlayerSpawn";

    [Header("Death Slow Motion")]
    [SerializeField, Range(0.02f, 1f)] private float deathTimeScale = 0.15f;

    private bool isDying = false;

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

            Debug.Log($"[GM] PlayerRef set to PlayerMovement root: {PlayerRef.name}");
            return;
        }

        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        Debug.LogWarning($"[GM] No encontr� PlayerMovement. Fallback PlayerRef={(PlayerRef ? PlayerRef.name : "NULL")}");
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

        float prevTimeScale = Time.timeScale;
        float prevFixedDelta = Time.fixedDeltaTime;

        SetPlayerControlEnabled(false);

        Time.timeScale = deathTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (deathEffect != null)
            yield return deathEffect.PlayDeathSequence();
        else
            Debug.LogWarning("[GM] deathEffect no asignado/encontrado.");

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        ResetLoops();
        if (loopManager != null)
            loopManager.StartNextLoop();

        TeleportPlayerToStart_Rigidbody();

        deathEffect?.Restore();
        SetPlayerControlEnabled(true);

        Time.timeScale = prevTimeScale;
        Time.fixedDeltaTime = prevFixedDelta;

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

    private void TeleportPlayerToStart_Rigidbody()
    {
        if (PlayerRef == null) return;

        GameObject sp = GameObject.FindGameObjectWithTag(playerSpawnTag);
        if (sp == null)
        {
            Debug.LogWarning($"No hay objeto con tag {playerSpawnTag} en la escena.");
            return;
        }

        Transform spawnPoint = sp.transform;

        Rigidbody rb = PlayerRef.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.position = spawnPoint.position;
            rb.rotation = spawnPoint.rotation;
        }
        else
        {
            PlayerRef.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        }
    }
}
