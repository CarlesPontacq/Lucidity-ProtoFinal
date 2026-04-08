using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    enum SpawnMethod { Front, SpawnPoints };

    [Header("EnemyStun")]
    [SerializeField] private bool disablePermanentlyWhenCaptured = false;
    [SerializeField] private float respawnDelay = 10f;

    [Header("Prefab")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Loop Rules")]
    private int baseLoop = 0;
    [SerializeField] private int firstSpawnLoop = 1;

    [Header("Spawn Position")]
    [SerializeField] private float spawnYOffset = 0.05f;

    [Header("Spawn Points")]
    [SerializeField] private float minDistanceFromPlayer = 4f;
    [SerializeField] private Transform[] spawnPoints;

    [Header("SFX Spawn")]
    [SerializeField] private string spawnSFX = "enemySpawn";
    [SerializeField] private float sfxVolume = 1.0f;

    private GameObject currentEnemy;
    private Coroutine spawnCycleRoutine;
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

    private void SpawnEnemyOnce()
    {
        if (currentEnemy != null)
        {
            Debug.LogWarning("[EnemySpawner] Intento de spawnear cuando ya hay un enemigo");
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogWarning("[EnemySpawner] enemyPrefab no asignado");
            return;
        }

        Transform player = GetPlayerTransform();
        if (player == null)
        {
            Debug.LogError("[EnemySpawner] No player reference found!");
            return;
        }
        
        Vector3 spawnPos = GetSpawnPosition(player);
        Debug.Log($"[EnemySpawner] Spawning enemy at position: {spawnPos}");

        currentEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        SFXManager.Instance.PlaySpatialSound(spawnSFX, currentEnemy.transform.position, sfxVolume);

        // Verificar que el enemigo se instanció correctamente
        if (currentEnemy == null)
        {
            Debug.LogError("[EnemySpawner] Failed to instantiate enemy prefab!");
            return;
        }

        Debug.Log($"[EnemySpawner] Enemy instantiated successfully: {currentEnemy.name}");

        // Mirar al player
        Vector3 lookDir = player.position - currentEnemy.transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
            currentEnemy.transform.rotation = Quaternion.LookRotation(lookDir);

        Rigidbody rb = currentEnemy.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.interpolation = RigidbodyInterpolation.Interpolate; 
        }

        Collider col = currentEnemy.GetComponentInChildren<Collider>();
        if (col != null)
        {
            Vector3 pos = currentEnemy.transform.position;
            pos.y += col.bounds.extents.y + spawnYOffset;
            currentEnemy.transform.position = pos;
        }

        EnemyFollowSteering follow = currentEnemy.GetComponent<EnemyFollowSteering>();
        if (follow != null)
            follow.SetCanChase(true);

        EnemyStunner stunner = currentEnemy.GetComponent<EnemyStunner>();
        if (stunner != null)
            stunner.Init(this);

        Debug.Log($"[EnemySpawner] Spawned enemy successfully. loop={currentLoopIndex} chase={true} asAnomaly={spawnedAsAnomaly}");
    }

    private Transform GetPlayerTransform()
    {
        return GameManager.PlayerRef != null ? GameManager.PlayerRef.transform : null;
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
            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator DelayedClear()
    {
        yield return new WaitForSeconds(0.1f);
        ClearEnemy();
    }

    private void ClearEnemy()
    {
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
            currentEnemy = null;
        }
    }

    private void StopCycle()
    {
        if (spawnCycleRoutine != null)
        {
            StopCoroutine(spawnCycleRoutine);
            spawnCycleRoutine = null;
        }
    }

    private Vector3 GetSpawnPosition(Transform player)
    {      
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[EnemySpawner] No spawn points assigned");
            return player.position;
        }

        List<Transform> validPoints = new List<Transform>();

        foreach (var point in spawnPoints)
        {
            float distance = Vector3.Distance(player.position, point.position);

            if (distance >= minDistanceFromPlayer)
                validPoints.Add(point);
        }

        if (validPoints.Count == 0)
        {
            Debug.LogWarning("[EnemySpawner] No valid spawn points far enough from player");
            return player.position;
        }

        int index = Random.Range(0, validPoints.Count);
        return validPoints[index].position;
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        SpawnEnemyOnce();
    }

    public void ResetCurrentLoopIndex()
    {
        currentLoopIndex = baseLoop;
    }
}