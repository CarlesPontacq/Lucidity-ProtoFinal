using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Loop Rules")]
    [SerializeField] private int firstSpawnLoop = 1;     
    [SerializeField] private int firstChaseLoop = 5;     
    [SerializeField, Range(0f, 1f)] private float spawnChancePerLoop = 0.65f;

    [Header("Front Spawn")]
    [SerializeField] private Transform forwardReference; 
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float minSpawnDistance = 3.5f;
    [SerializeField] private float maxSpawnDistance = 6f;
    [SerializeField] private float lateralOffset = 1.5f;
    [SerializeField] private float groundRayHeight = 15f;
    [SerializeField] private float groundRayDistance = 50f;
    [SerializeField] private float spawnYOffset = 0.05f;

    [Header("Cycle Timing")]
    [SerializeField] private float visibleTimeMin = 1.0f;
    [SerializeField] private float visibleTimeMax = 2.0f;
    [SerializeField] private float hiddenTimeMin = 1.2f;
    [SerializeField] private float hiddenTimeMax = 2.5f;

    private GameObject currentEnemy;
    private Coroutine spawnCycleRoutine;
    private bool enemyEnabledThisLoop = false;
    private int currentLoopIndex = 0;

    public void SpawnForLoop(int loopIndex)
    {
        currentLoopIndex = loopIndex;

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

    private IEnumerator SpawnCycleRoutine()
    {
        while (enemyEnabledThisLoop)
        {
            SpawnEnemyOnce();

            float visibleTime = Random.Range(visibleTimeMin, visibleTimeMax);
            yield return new WaitForSeconds(visibleTime);

            ClearEnemy();

            float hiddenTime = Random.Range(hiddenTimeMin, hiddenTimeMax);
            yield return new WaitForSeconds(hiddenTime);
        }
    }

    private void SpawnEnemyOnce()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("[EnemySpawner] enemyPrefab no asignado");
            return;
        }

        Transform player = GetPlayerTransform();
        if (player == null)
            return;

        Vector3 spawnPos = GetPointInFrontOfPlayer(player);
        currentEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // Mirar al player
        Vector3 lookDir = player.position - currentEnemy.transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
            currentEnemy.transform.rotation = Quaternion.LookRotation(lookDir);

        bool shouldChase = currentLoopIndex >= firstChaseLoop;

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

        Debug.Log($"[EnemySpawner] Spawned enemy. loop={currentLoopIndex} chase={shouldChase}");
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
        ClearEnemy();
        enemyEnabledThisLoop = false;
        StopCycle();
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
}