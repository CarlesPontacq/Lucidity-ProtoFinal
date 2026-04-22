using UnityEngine;

public class FirstChaseEnabler : MonoBehaviour
{
    [Header("First Enemy Chase")]
    [SerializeField] private GameObject firstChaseEnemy;
    [SerializeField] private EnemyFollowSteering enemyFollow;
    [SerializeField] private bool firstChase;
    [SerializeField] private BoxCollider firstChaseTrigger;

    [SerializeField] private DoorInteraction door;


    private string playerTag = "Player";


    private void Awake()
    {
        door.Lock();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == playerTag && firstChase)
        {
            firstChase = false;
            firstChaseEnemy.SetActive(true);
            enemyFollow.SetCanChase(true);

            firstChaseTrigger.enabled = true;
            door.Unlock();
        }
    }
}
