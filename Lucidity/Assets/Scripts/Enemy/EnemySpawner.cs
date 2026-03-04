using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private GameObject currentEnemy;

    [SerializeField] private float respawnDelay = 2f;
    public void SpawnForNewLoop()
    {
        int currentLoop = GameManager.Instance.GetCurrentLoopIndex();

        if (currentLoop <= 0)
        {
            ClearEnemy();
            Debug.Log("[EnemySpawner] Loop 0 -> no enemy");
            return;
        }

        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        ClearEnemy();

        if (enemyPrefab == null || spawnPoints.Length == 0)
            return;

        int index = Random.Range(0, spawnPoints.Length);
        Transform point = spawnPoints[index];

        currentEnemy = Instantiate(enemyPrefab, point.position, point.rotation);

        var stunner = currentEnemy.GetComponent<EnemyStunner>();
        if (stunner != null)
            stunner.Init(this);
        enemyPrefab.GetComponent<EnemyKillOnTouch>().triggered = false;

        Debug.Log("[EnemySpawner] Spawned enemy at " + point.name);
    }

    private void ClearEnemy()
    {
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
            currentEnemy = null;
        }
    }

    public void OnEnemyCaptured()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        SpawnForNewLoop();
    }
}