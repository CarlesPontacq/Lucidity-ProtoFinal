using Unity.VisualScripting;
using UnityEngine;

public class StartEnemyChaseSequance : MonoBehaviour
{
    [SerializeField] private GameObject firstChaseEnemy;
    [SerializeField] private EnemyChaseSpawner chaseSpawner;
    [SerializeField] private DoorController door;

    private string playerTag = "Player";
    private int floor = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == playerTag)
        {
            if(firstChaseEnemy != null) 
            { 
                Destroy(firstChaseEnemy);
                chaseSpawner.enabled = true;
                chaseSpawner.currentFloor = floor;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        chaseSpawner.DestroyCurrentEnemy();

        if (door != null)
            door.Open(false);

        chaseSpawner.currentFloor = floor;
    }
}
