using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs / Normal Spawn")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Loop Rules")]
    [SerializeField] private int firstSpawnLoop = 1;
    [SerializeField] private int firstChaseLoop = 5;

    [Header("Psychological Spawn (loops 1-4)")]
    [SerializeField] private float firstSpawnDistance = 12f;
    [SerializeField] private float finalSpawnDistance = 4f;
    [SerializeField] private float lateralOffset = 1.5f;
    [SerializeField] private float vanishDelay = 1.0f;
    [SerializeField] private float respawnDelay = 2.0f;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundRayHeight = 15f;
    [SerializeField] private float groundRayDistance = 50f;
    [SerializeField] private float spawnYOffset = 0.1f;

    [Header("Front Spawn References")]
    [SerializeField] private Transform forwardReference;  
    [SerializeField] private BoxCollider playableArea;     

    private GameObject currentEnemy;
    private Coroutine psychologicalRoutine;

    public void SpawnForLoop(int loopIndex)
    {
        if (loopIndex < firstSpawnLoop)
        {
            StopPsychologicalRoutine();
            ClearEnemy();
            Debug.Log($"[EnemySpawner] loop {loopIndex} -> no enemy");
            return;
        }

        if (loopIndex < firstChaseLoop)
        {
            StopPsychologicalRoutine();
            ClearEnemy();
            psychologicalRoutine = StartCoroutine(PsychologicalSpawnRoutine(loopIndex));
            return;
        }

        StopPsychologicalRoutine();
        SpawnNormalEnemy(loopIndex);
    }

    private IEnumerator PsychologicalSpawnRoutine(int loopIndex)
    {
        Transform player = GetPlayerTransform();
        if (player == null)
            yield break;

        Vector3 firstPos = GetPointInFrontOfPlayer(player, firstSpawnDistance);
        SpawnEnemyAt(firstPos, loopIndex, false);

        yield return new WaitForSeconds(vanishDelay);

        ClearEnemy();

        yield return new WaitForSeconds(respawnDelay);

        player = GetPlayerTransform();
        if (player == null)
            yield break;

        float t = Mathf.InverseLerp(firstSpawnLoop, firstChaseLoop - 1, loopIndex);
        float closerDistance = Mathf.Lerp(firstSpawnDistance, finalSpawnDistance, t);

        Vector3 secondPos = GetPointInFrontOfPlayer(player, closerDistance);
        SpawnEnemyAt(secondPos, loopIndex, false);
    }

    private void SpawnNormalEnemy(int loopIndex)
    {
        ClearEnemy();

        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[EnemySpawner] Falta enemyPrefab o spawnPoints");
            return;
        }

        int index = Random.Range(0, spawnPoints.Length);
        Transform point = spawnPoints[index];

        Vector3 pos = point.position + Vector3.up * spawnYOffset;
        SpawnEnemyAt(pos, loopIndex, true);

        Debug.Log($"[EnemySpawner] Normal spawn at {point.name}. loop={loopIndex}");
    }
    private void SpawnEnemyAt(Vector3 worldPos, int loopIndex, bool shouldChase)
    {
        if (enemyPrefab == null) return;

        ClearEnemy();

        currentEnemy = Instantiate(enemyPrefab, worldPos, Quaternion.identity);

        // Ajustar altura real según collider
        Collider col = currentEnemy.GetComponentInChildren<Collider>();
        if (col != null)
        {
            Vector3 pos = currentEnemy.transform.position;
            pos.y += col.bounds.extents.y;
            currentEnemy.transform.position = pos;
        }

        Transform player = GetPlayerTransform();
        if (player != null)
        {
            Vector3 lookDir = player.position - currentEnemy.transform.position;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.001f)
                currentEnemy.transform.rotation = Quaternion.LookRotation(lookDir);
        }

        EnemyFollowSteering follow = currentEnemy.GetComponent<EnemyFollowSteering>();
        if (follow != null)
            follow.SetCanChase(shouldChase);

        EnemyStunner stunner = currentEnemy.GetComponent<EnemyStunner>();
        if (stunner != null)
            stunner.Init(this);
    }

    private Vector3 GetPointInFrontOfPlayer(Transform player, float distance)
    {
        // 1) Dirección real: cámara si existe, si no player
        Transform dirSource = forwardReference != null ? forwardReference : player;

        Vector3 forward = dirSource.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = dirSource.right;
        right.y = 0f;
        right.Normalize();

        float randomSide = Random.Range(-lateralOffset, lateralOffset);

        Vector3 flatTarget = player.position + forward * distance + right * randomSide;

        // 2) Clamp dentro del área jugable
        if (playableArea != null)
        {
            Bounds b = playableArea.bounds;

            flatTarget.x = Mathf.Clamp(flatTarget.x, b.min.x, b.max.x);
            flatTarget.z = Mathf.Clamp(flatTarget.z, b.min.z, b.max.z);
        }

        // 3) Buscar el suelo justo debajo de ese punto
        Vector3 rayOrigin = new Vector3(flatTarget.x, flatTarget.y + groundRayHeight, flatTarget.z);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundRayDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }

        Debug.LogWarning($"[EnemySpawner] No encontré suelo delante del jugador. Uso fallback.");
        return new Vector3(flatTarget.x, player.position.y, flatTarget.z);
    }

    private Transform GetPlayerTransform()
    {
        return GameManager.PlayerRef != null ? GameManager.PlayerRef.transform : null;
    }

    public void OnEnemyCaptured()
    {
        currentEnemy = null;
    }

    private void ClearEnemy()
    {
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
            currentEnemy = null;
        }
    }

    private void StopPsychologicalRoutine()
    {
        if (psychologicalRoutine != null)
        {
            StopCoroutine(psychologicalRoutine);
            psychologicalRoutine = null;
        }
    }
}