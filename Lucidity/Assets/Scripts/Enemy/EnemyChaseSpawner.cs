using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class EnemyChaseSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float enemyChaseSpeed = 3f;

    [Header("Spawn Position")]
    [SerializeField] private float spawnYOffset = 0.05f;
    [SerializeField] public int currentFloor = 1;

    [Header("Spawn Points")]
    [SerializeField] private float minDistanceFromPlayer = 2f;
    [SerializeField] private float maxDistanceFromPlayer = 7f;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Spawn Cycle")]
    [SerializeField] private float enemyLifetime = 5f;
    [SerializeField] private float maxDistanceBetweenEnemyAndPlayer = 10f;

    [Header("SFX Spawn")]
    [SerializeField] private string spawnSFX = "enemySpawn";
    [SerializeField] private float sfxVolume = 1.0f;

    private GameObject currentEnemy;
    private Coroutine spawnCycleCoroutine;
    private bool isSpawning = true;

    void Start()
    {
        StartSpawnCycle();
    }

    void Update()
    {
        if(currentEnemy != null)
        {
            float distance = Vector3.Distance(playerTransform.position, currentEnemy.transform.position);
            if (distance > maxDistanceBetweenEnemyAndPlayer)
            {
                ResetSpawnCycle();
            }
        }
    }

    private void OnEnable()
    {
        if (spawnCycleCoroutine != null)
            StopCoroutine(spawnCycleCoroutine);

        StartSpawnCycle();
    }

    private void OnDisable()
    {
        if (spawnCycleCoroutine != null)
            StopCoroutine(spawnCycleCoroutine);

        DestroyCurrentEnemy();
    }

    private void StartSpawnCycle()
    {
        if (spawnCycleCoroutine != null)
            StopCoroutine(spawnCycleCoroutine);

        spawnCycleCoroutine = StartCoroutine(SpawnCycleCoroutine());
    }

    private IEnumerator SpawnCycleCoroutine()
    {
        while (isSpawning)
        {
            if(currentEnemy == null)
                SpawnEnemyOnce();

            if (currentEnemy != null)
            {
                yield return new WaitForSeconds(enemyLifetime);
                DestroyCurrentEnemy();
            }
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

        if (playerTransform == null)
        {
            Debug.LogError("[EnemySpawner] No player reference found!");
            return;
        }

        Vector3 spawnPos = GetSpawnPosition(playerTransform);
        Debug.Log($"[EnemySpawner] Spawning enemy at position: {spawnPos}");

        currentEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        if(SFXManager.Instance != null)
            SFXManager.Instance.PlaySpatialSound(spawnSFX, currentEnemy.transform.position, sfxVolume);

        // Verificar que el enemigo se instanció correctamente
        if (currentEnemy == null)
        {
            Debug.LogError("[EnemySpawner] Failed to instantiate enemy prefab!");
            return;
        }

        Debug.Log($"[EnemySpawner] Enemy instantiated successfully: {currentEnemy.name}");

        // Mirar al player
        Vector3 lookDir = playerTransform.position - currentEnemy.transform.position;
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
        {
            follow.SetCanChase(true);
            follow.SetChaseSpeed(enemyChaseSpeed);
        }
    }

    public void DestroyCurrentEnemy()
    {
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
            currentEnemy = null;
        }
    }

    public void ResetSpawnCycle()
    {
        DestroyCurrentEnemy();
        StartSpawnCycle();
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
            if(!point.gameObject.activeSelf) continue;

            float distance = Vector3.Distance(player.position, point.position);

            if (distance > minDistanceFromPlayer && distance < maxDistanceFromPlayer)
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
}
