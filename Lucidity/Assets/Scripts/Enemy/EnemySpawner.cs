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
    [SerializeField] private int firstChaseLoop = 5;
    [SerializeField, Range(0f, 1f)] private float spawnChancePerLoop = 0.65f;

    [Header("Spawn Position")]
    [SerializeField] private SpawnMethod spawnMethod;
    [SerializeField] private Transform forwardReference;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float minSpawnDistance = 3.5f;
    [SerializeField] private float maxSpawnDistance = 6f;
    [SerializeField] private float lateralOffset = 1.5f;
    [SerializeField] private float groundRayHeight = 15f;
    [SerializeField] private float groundRayDistance = 50f;
    [SerializeField] private float spawnYOffset = 0.05f;

    [Header("Spawn Points")]
    [SerializeField] private float minDistanceFromPlayer = 4f;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Cycle Timing")]
    [SerializeField] private float visibleTimeMin = 1.0f;
    [SerializeField] private float visibleTimeMax = 2.0f;
    [SerializeField] private float hiddenTimeMin = 1.2f;
    [SerializeField] private float hiddenTimeMax = 2.5f;

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

    public void SpawnForLoop(int loopIndex)
    {
        currentLoopIndex = loopIndex;
        spawnedAsAnomaly = false;

        StopCycle();
        ClearEnemy();

        if (loopIndex < firstSpawnLoop)
        {
            enemyEnabledThisLoop = false;
            Debug.Log($"[EnemySpawner] loop {loopIndex} -> no enemy");
            return;
        }

        enemyEnabledThisLoop = Random.value <= spawnChancePerLoop;

        if (!enemyEnabledThisLoop)
        {
            Debug.Log($"[EnemySpawner] loop {loopIndex} -> enemy skipped");
            return;
        }

        spawnCycleRoutine = StartCoroutine(SpawnCycleRoutine());
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

        if (currentLoopIndex < firstChaseLoop)
        {
            spawnCycleRoutine = StartCoroutine(SpawnCycleRoutine());
        }
    }

    private IEnumerator SpawnCycleRoutine()
    {
        while (enemyEnabledThisLoop)
        {
            SpawnEnemyOnce();

            float visibleTime = Random.Range(visibleTimeMin, visibleTimeMax);
            yield return new WaitForSeconds(visibleTime);

            bool shouldChase = currentLoopIndex >= firstChaseLoop;
            if (shouldChase)
            {
                yield break;
            }

            ClearEnemy();

            float hiddenTime = Random.Range(hiddenTimeMin, hiddenTimeMax);
            yield return new WaitForSeconds(hiddenTime);
        }
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

        bool shouldChase = currentLoopIndex >= firstChaseLoop;

        // Use spawn points for anomaly spawns for better control
        if (spawnedAsAnomaly)
        {
            spawnMethod = SpawnMethod.SpawnPoints;
        }
        else
        {
            spawnMethod = shouldChase ? SpawnMethod.SpawnPoints : SpawnMethod.Front;
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
            if (shouldChase)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;
                rb.isKinematic = true;
            }
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
            follow.SetCanChase(shouldChase);

        EnemyStunner stunner = currentEnemy.GetComponent<EnemyStunner>();
        if (stunner != null)
            stunner.Init(this);

        Debug.Log($"[EnemySpawner] Spawned enemy successfully. loop={currentLoopIndex} chase={shouldChase} asAnomaly={spawnedAsAnomaly}");
    }

    private Vector3 GetPointInFrontOfPlayer(Transform player)
    {
        Transform dirSource = forwardReference != null ? forwardReference : player;

        Vector3 forward = dirSource.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = dirSource.right;
        right.y = 0f;
        right.Normalize();

        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
        float side = Random.Range(-lateralOffset, lateralOffset);

        Vector3 flatTarget = player.position + forward * distance + right * side;
        Vector3 rayOrigin = new Vector3(flatTarget.x, flatTarget.y + groundRayHeight, flatTarget.z);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundRayDistance, groundMask, QueryTriggerInteraction.Ignore))
            return hit.point;

        // fallback
        return new Vector3(flatTarget.x, player.position.y, flatTarget.z);
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
        switch (spawnMethod)
        {
            case SpawnMethod.Front:
                return GetPointInFrontOfPlayer(player);

            case SpawnMethod.SpawnPoints:

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

            default:
                return GetPointInFrontOfPlayer(player);
        }
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        SpawnEnemyOnce();
    }
}