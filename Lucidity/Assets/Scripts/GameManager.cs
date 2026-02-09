using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static GameObject PlayerRef { get; private set; }

    [Header("Loop")]
    [SerializeField] private LoopCounter loopCounterUI;
    [SerializeField] private LoopManager loopManager;

    [Header("Death FX")]
    [SerializeField] private DeathCameraEffect deathEffect;          
    [SerializeField] private MonoBehaviour[] disableOnDeath;       
    [SerializeField] private string playerSpawnTag = "PlayerSpawn";  

    [Header("Death Timing")]
    [SerializeField] private float extraDelayAfterFlash = 0.05f;     

    private int currentLoop = 0;
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

        if (deathEffect == null && PlayerRef != null)
            deathEffect = PlayerRef.GetComponentInChildren<DeathCameraEffect>();
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
        StartCoroutine(PlayerDiedRoutine());
    }

    private IEnumerator PlayerDiedRoutine()
    {
        isDying = true;

        Time.timeScale = 1f;

        SetPlayerControlEnabled(false);

        if (deathEffect != null)
            yield return deathEffect.PlayDeathSequence();
        else
            Debug.LogWarning("[GameManager] deathEffect no asignado/encontrado.");

        if (extraDelayAfterFlash > 0f)
            yield return new WaitForSecondsRealtime(extraDelayAfterFlash);

        ResetLoops();

        if (loopManager != null)
            loopManager.StartNextLoop();
        else
            Debug.LogWarning("[GameManager] loopManager es null.");

        TeleportPlayerToStart();

        deathEffect?.Restore();
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
