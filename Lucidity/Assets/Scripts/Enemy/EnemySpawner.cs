using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private LoopManager loopManager;     
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawnPoints = new();

    private GameObject currentEnemy;
    private int lastSpawnIndex = -1;

    private void Start()
    {
        SpawnForNewLoop(); 
    }

    public void SpawnForNewLoop()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("[EnemySpawner] enemyPrefab es null.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("[EnemySpawner] No hay spawnPoints.");
            return;
        }

        if (currentEnemy != null)
            Destroy(currentEnemy);

        int index = Random.Range(0, spawnPoints.Count);
        if (spawnPoints.Count > 1)
        {
            while (index == lastSpawnIndex)
                index = Random.Range(0, spawnPoints.Count);
        }
        lastSpawnIndex = index;

        Transform sp = spawnPoints[index];
        currentEnemy = Instantiate(enemyPrefab, sp.position, sp.rotation);

        Debug.Log($"[EnemySpawner] Spawned enemy at {sp.name}");
    }

    public void ClearEnemy()
    {
        if (currentEnemy != null)
            Destroy(currentEnemy);
        currentEnemy = null;
        lastSpawnIndex = -1;
    }
}
