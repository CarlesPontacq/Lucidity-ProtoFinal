using UnityEngine;

public class MoveEnemySpawnsFloor : MonoBehaviour
{
    [SerializeField] EnemyChaseSpawner enemyChaseSpawner;
    [SerializeField] private Transform enemySpawnsParent;
    [SerializeField] private float newYPos;
    private string playerTag = "Player";

    public int floor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == playerTag && enemyChaseSpawner.currentFloor != floor)
        {
            Vector3 newPos = enemySpawnsParent.position;
            newPos.y = newYPos;
            enemySpawnsParent.position = newPos;

            enemyChaseSpawner.ResetSpawnCycle();

            enemyChaseSpawner.currentFloor = floor;
        }
    }
}
