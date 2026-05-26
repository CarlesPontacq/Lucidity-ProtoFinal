using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseSpawner : EnemySpawner
{
    [Header("Prefab")]
    [SerializeField] private float enemyChaseSpeed = 3f;

    [Header("Spawn Position")]
    [SerializeField] public int currentFloor = 1;

    [Header("Spawn Points")]
    [SerializeField] private float maxDistanceFromPlayer = 7f;

    [Header("Spawn Cycle")]
    [SerializeField] private float enemyLifetime = 5f;
    [SerializeField] private float maxDistanceBetweenEnemyAndPlayer = 10f;

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

    private void OnDisable()
    {
        if (spawnCycleRoutine != null)
            StopCoroutine(spawnCycleRoutine);

        DestroyCurrentEnemy();
    }

    public void StartSpawnCycle()
    {
        if (spawnCycleRoutine != null)
            StopCoroutine(spawnCycleRoutine);

        spawnCycleRoutine = StartCoroutine(SpawnCycleCoroutine());
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

    public void DestroyCurrentEnemy()
    {
        if (spawnCycleRoutine != null)
            StopCoroutine(spawnCycleRoutine);

        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
            currentEnemy = null;
        }

        playerEnemyDetection.SetEnemy(null);
    }

    public void ResetSpawnCycle()
    {
        DestroyCurrentEnemy();
        StartSpawnCycle();
    }

    protected override bool CheckIfSpawnPointIsValid(float distance)
    {
        return distance > minDistanceFromPlayer && distance < maxDistanceFromPlayer;
    }

    public void SetDefaultSpawnPoint(Transform newDefault)
    {
        defaultSpawnPoint = newDefault;
    }
}