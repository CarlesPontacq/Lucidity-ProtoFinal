using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLoopSpawner : EnemySpawner
{
    [Header("EnemyStun")]
    [SerializeField] private bool disablePermanentlyWhenCaptured = false;
    [SerializeField] private float respawnDelay = 10f;

    [Header("Loop Rules")]
    private int baseLoop = 0;
    [SerializeField] private int firstSpawnLoop = 1;

    [Header("Spawn Effects")]
    [SerializeField] private CameraUIHandler cameraUI;

    public bool enemyEnabledThisLoop { get; private set; } = false;
    private int currentLoopIndex = 0;
    private bool spawnedAsAnomaly = false;

    private void Update()
    {
        if (currentLoopIndex == baseLoop && currentEnemy != null)
        {
            ClearEnemy();
        }
    }

    public void SpawnForLoopAsAnomaly(int loopIndex)
    {
        currentLoopIndex = loopIndex;
        spawnedAsAnomaly = true;
        enemyEnabledThisLoop = true;

        StopCycle();
        ClearEnemy();

        Debug.Log($"[EnemySpawner] Spawning enemy as anomaly for loop {currentLoopIndex}");

        SpawnEnemyOnce();
    }

    protected override void SpawnEnemyOnce()
    {
        base.SpawnEnemyOnce();

        EnemyStunner stunner = currentEnemy.GetComponent<EnemyStunner>();
        if (stunner != null)
            stunner.Init(this);

        Debug.Log($"[EnemySpawner] Spawned enemy successfully. loop={currentLoopIndex} chase={true} asAnomaly={spawnedAsAnomaly}");

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
}