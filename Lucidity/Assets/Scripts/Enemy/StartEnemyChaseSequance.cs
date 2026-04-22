using Unity.VisualScripting;
using UnityEngine;

public class StartEnemyChaseSequance : MonoBehaviour
{
    [SerializeField] private GameObject firstChaseEnemy;
    [SerializeField] private EnemyChaseSpawner chaseSpawner;
    private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == playerTag)
        {
            if(firstChaseEnemy != null) 
            { 
                Destroy(firstChaseEnemy);
                chaseSpawner.enabled = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        chaseSpawner.DestroyCurrentEnemy();
    }
}
