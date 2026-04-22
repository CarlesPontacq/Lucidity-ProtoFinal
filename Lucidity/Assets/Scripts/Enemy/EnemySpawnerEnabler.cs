using UnityEngine;

public class EnemySpawnerEnabler : MonoBehaviour
{
    [SerializeField] private EnemyChaseSpawner enemyChaseSpawner;
    private string playerTag = "Player";
    [SerializeField] private bool enableEnemySpawner;
    [SerializeField] private DoorInteraction  door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == playerTag && enemyChaseSpawner.enabled != enableEnemySpawner)
        {
            enemyChaseSpawner.enabled = enableEnemySpawner;

            if(door != null)
            {
                door.Lock();
                door.Close(true);
            }
        }

    }
}
