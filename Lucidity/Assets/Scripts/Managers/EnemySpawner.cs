using UnityEditor.Rendering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected GameObject enemyPrefab;
    protected Transform playerTransform;

    [Header("Spawn Position")]
    [SerializeField] protected float spawnYOffset = 0.05f;
    [SerializeField] protected float minDistanceFromPlayer = 4f;
    [SerializeField] protected Transform[] spawnPoints;
    [SerializeField] protected Transform defaultSpawnPoint;

    [Header("Spawn Effects")]
    [SerializeField] protected string spawnSFX = "enemySpawn";
    [SerializeField] protected float sfxVolume = 1.0f;

    [Header("ProximityEffect")]
    [SerializeField] protected PlayerEnemyDetection playerEnemyDetection;

    protected GameObject currentEnemy;
    protected Coroutine spawnCycleRoutine;

    protected virtual void SpawnEnemyOnce()
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
            playerTransform = GameManager.PlayerRef.transform;
        }

        Vector3 spawnPos = GetSpawnPosition(playerTransform);
        Debug.Log($"[EnemySpawner] Spawning enemy at position: {spawnPos}");

        currentEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        if (SFXManager.Instance != null)
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

        Collider col = currentEnemy.GetComponentInChildren<Collider>();
        if (col != null)
        {
            Vector3 pos = currentEnemy.transform.position;
            pos.y += col.bounds.extents.y + spawnYOffset;
            currentEnemy.transform.position = pos;
        }

        playerEnemyDetection.SetEnemy(currentEnemy);
    }

    protected Vector3 GetSpawnPosition(Transform player)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[EnemySpawner] No spawn points assigned");
            return defaultSpawnPoint.position;
        }

        List<Transform> validPoints = new List<Transform>();

        foreach (var point in spawnPoints)
        {
            if (!point.gameObject.activeSelf) continue;

            float distance = Vector3.Distance(player.position, point.position);

            if (CheckIfSpawnPointIsValid(distance))
                validPoints.Add(point);
        }

        if (validPoints.Count == 0)
        {
            Debug.LogWarning("[EnemySpawner] No valid spawn points far enough from player");
            return defaultSpawnPoint.position;
        }

        int index = Random.Range(0, validPoints.Count);
        return validPoints[index].position;
    }

    protected virtual bool CheckIfSpawnPointIsValid(float distance)
    {
        return distance >= minDistanceFromPlayer;
    }   
}