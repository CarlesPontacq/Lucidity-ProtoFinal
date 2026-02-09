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
    [SerializeField] private DeathCameraEffect deathEffect;        // en la cámara del player
    [SerializeField] private MonoBehaviour[] disableOnDeath;       // scripts de input/mov/cámara/interacción
    [SerializeField] private string playerSpawnTag = "PlayerSpawn";

    [Header("Death Slow Motion")]
    [SerializeField, Range(0.02f, 1f)] private float deathTimeScale = 0.2f;

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
        PlayerRef = GameObject.FindGameObjectWithTag("Player");

        // fallback si no lo asignaste
        if (deathEffect == null && PlayerRef != null)
            deathEffect = PlayerRef.GetComponentInChildren<DeathCameraEffect>(true);
    }

    public void ResetAndStartNextLoop()
    {
        Debug.Log("ResetAndStartNextLoop se ejecuta");
        loopManager.StartNextLoop();
    }

    public int GetCurrentLoopIndex()
    {
        return currentLoop;
    }

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
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        isDying = true;

        // Guardar estado de tiempo
        float prevTimeScale = Time.timeScale;
        float prevFixedDelta = Time.fixedDeltaTime;

        // 1) bloquear control del jugador (pantalla quieta)
        SetPlayerControlEnabled(false);

        // 2) activar slowmo del mundo
        Time.timeScale = deathTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // 3) efectos (cámara + flash) en tiempo real (no slowmo)
        if (deathEffect != null)
            yield return deathEffect.PlayDeathSequence();
        else
            Debug.LogWarning("[GameManager] deathEffect no asignado/encontrado.");

        // 4) volver a velocidad normal ANTES de respawn/restart
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // 5) reset del juego
        ResetLoops();
        if (loopManager != null)
            loopManager.StartNextLoop();

        TeleportPlayerToStart();

        // 6) restaurar FX y control
        deathEffect?.Restore();
        SetPlayerControlEnabled(true);

        // 7) restaurar por si antes del death usabas otro timeScale
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

    private void TeleportPlayerToStart()
    {
        GameObject player = PlayerRef;
        if (player == null) return;

        GameObject sp = GameObject.FindGameObjectWithTag(playerSpawnTag);
        if (sp == null)
        {
            Debug.LogWarning($"No hay objeto con tag {playerSpawnTag} en la escena.");
            return;
        }

        Transform spawnPoint = sp.transform;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;

        if (cc != null) cc.enabled = true;
    }
}
