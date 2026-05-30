using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLoopSpawner : EnemySpawner
{
    [Header("EnemyStun")]
    [SerializeField] private bool disablePermanentlyWhenCaptured = false;
    [SerializeField] private float respawnDelay = 10f;

    [Header("Loop Rules")]
    [SerializeField] private int firstSpawnLoop = 1;
    private int baseLoop = 0;
    [SerializeField][Range(0f, 1f)] private float enemySpawnProbability = 0.6f;
    [SerializeField] private float stunUnlockSpawnDelay = 1.5f;

    [Header("Scripted Loops")]
    [SerializeField] private List<ScriptedLoopEnemy> scriptedLoopsEnemies = new();

    [Header("Spawn Effects")]
    [SerializeField] private CameraUIHandler cameraUI;

    [System.Serializable]
    public class ScriptedLoopEnemy
    {
        public int loop;
        public bool loopHasBeenProcessed;
        public bool enemyHasToSpawn;
    }

    public bool enemyEnabledThisLoop { get; private set; } = false;
    private int currentLoopIndex = 0;
    private bool pendingEnemySpawn = false;
    private bool enemyHasToSpawnForCurrentLoop = false;

    private void Start()
    {
        GameManager.GrabHandlerRef.OnStunUnlocked += HandleStunUnlocked;
    }

    private void Update()
    {
        if (currentLoopIndex == baseLoop && currentEnemy != null)
        {
            ClearEnemy();
        }
    }

    public void DecideAndSpawnForLoop(int loopIndex)
    {
        currentLoopIndex = loopIndex;

        // Verificar si hay regla para loop scripteado
        if (currentLoopIndex - 1 < scriptedLoopsEnemies.Count)
        {
            var scriptedLoop = scriptedLoopsEnemies[currentLoopIndex - 1];
            if (!scriptedLoop.loopHasBeenProcessed)
            {
                scriptedLoop.loopHasBeenProcessed = true;
                enemyHasToSpawnForCurrentLoop = scriptedLoop.enemyHasToSpawn;

                if (enemyHasToSpawnForCurrentLoop)
                {
                    SpawnEnemyIfNeeded();
                }
                return;
            }
        }

        // Decisión aleatoria para loops no scripteados
        DecideIfEnemySpawnsRandomly();
        SpawnEnemyIfNeeded();
    }

    private void DecideIfEnemySpawnsRandomly()
    {
        enemyHasToSpawnForCurrentLoop = UnityEngine.Random.value <= enemySpawnProbability;

        if (enemyHasToSpawnForCurrentLoop)
        {
            Debug.Log($"[EnemyLoopSpawner] Enemy will spawn this loop with probability {enemySpawnProbability:P}");
        }
        else
        {
            Debug.Log($"[EnemyLoopSpawner] No enemy this loop");
        }
    }

    public void SpawnForLoopAsAnomaly(int loopIndex)
    {
        currentLoopIndex = loopIndex;
        enemyEnabledThisLoop = true;

        StopCycle();
        ClearEnemy();

        Debug.Log($"[EnemySpawner] Spawning enemy as anomaly for loop {currentLoopIndex}");

        SpawnEnemyOnce();
    }

    private void SpawnEnemyIfNeeded()
    {
        if (!enemyHasToSpawnForCurrentLoop)
            return;

        if (GameManager.GrabHandlerRef.HasUnlockedStun())
        {
            Debug.Log($"[EnemyLoopSpawner] Spawning enemy normally for loop {currentLoopIndex}");
            SpawnForLoopAsAnomaly(currentLoopIndex);
        }
        else
        {
            Debug.Log("[EnemyLoopSpawner] Enemy waiting for stun unlock");
            pendingEnemySpawn = true;
        }
    }

    protected override void SpawnEnemyOnce()
    {
        base.SpawnEnemyOnce();

        EnemyStunner stunner = currentEnemy.GetComponent<EnemyStunner>();
        if (stunner != null)
            stunner.Init(this);

        Debug.Log($"[EnemySpawner] Spawned enemy successfully. loop={currentLoopIndex} chase={true}");

        cameraUI.ShowCameraRedLight(true);
    }

    public void OnEnemyCaptured()
    {
        if (currentEnemy != null)
        {
            if (currentEnemy.gameObject != null)
            {
                StartCoroutine(DelayedClear());
            }
        }

        if (disablePermanentlyWhenCaptured)
        {
            enemyEnabledThisLoop = false;
            StopCycle();
        }
        else
        {
           spawnCycleRoutine = StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator DelayedClear()
    {
        yield return new WaitForSeconds(0.1f);
        ClearEnemy();
    }

    public void ClearEnemy()
    {
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
            currentEnemy = null;

            playerEnemyDetection.SetEnemy(null);
        }
            cameraUI.ShowCameraRedLight(false);
    }

    private void StopCycle()
    {
        if (spawnCycleRoutine != null)
        {
            StopCoroutine(spawnCycleRoutine);
            spawnCycleRoutine = null;
        }
    }

    private IEnumerator RespawnRoutine()
    {
        Debug.Log("Anomalia: Repawn Iniciado");
        yield return new WaitForSeconds(respawnDelay);

        Debug.Log("Anomalia: Repawn Completado");
        SpawnEnemyOnce();
    }

    public void ResetCurrentLoopIndex()
    {
        currentLoopIndex = baseLoop;
    }

    private void HandleStunUnlocked()
    {
        if (!pendingEnemySpawn)
            return;

        pendingEnemySpawn = false;

        StartCoroutine(DelayedEnemySpawnAfterUnlock());
    }

    private IEnumerator DelayedEnemySpawnAfterUnlock()
    {
        yield return new WaitForSeconds(stunUnlockSpawnDelay);

        if (enemyHasToSpawnForCurrentLoop)
        {
            Debug.Log("[EnemyLoopSpawner] Spawning delayed enemy after stun unlock");
            SpawnForLoopAsAnomaly(currentLoopIndex);
        }
    }

    public bool DoesEnemySpawnThisLoop() => enemyHasToSpawnForCurrentLoop;

    private void OnDestroy()
    {
        if (GameManager.GrabHandlerRef != null)
            GameManager.GrabHandlerRef.OnStunUnlocked -= HandleStunUnlocked;
    }
}