using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static GameObject PlayerRef { get; private set; }

    [SerializeField] private LoopCounter loopCounterUI;
    [SerializeField] private LoopManager loopManager;
    private int currentLoop = 0;

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
        Debug.Log("Player murio -> reset loops y reinicio loop");

        ResetLoops();                 // contador a 0
        loopManager.StartNextLoop();  // reinicia sistema de loop
        TeleportPlayerToStart();
    }

    private void TeleportPlayerToStart()
    {
        GameObject player = PlayerRef;
        if (player == null) return;

        Transform spawnPoint = GameObject.FindWithTag("PlayerSpawn")?.transform;
        if (spawnPoint == null)
        {
            Debug.LogWarning("No hay PlayerSpawn en la escena.");
            return;
        }

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;

        if (cc != null)
            cc.enabled = true;
    }

}
