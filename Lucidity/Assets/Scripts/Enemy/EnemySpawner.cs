using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private GameObject currentEnemy;

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
}