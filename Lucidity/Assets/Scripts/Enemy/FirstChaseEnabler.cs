using UnityEngine;

public class FirstChaseEnabler : MonoBehaviour
{
    [SerializeField] private GameObject firstChaseEnemy;
    [SerializeField] private EnemyFollowSteering enemyFollow;
    [SerializeField] private bool firstChase;

    [SerializeField] private BoxCollider firstChaseTrigger;
    private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == playerTag && firstChase)
        {
            firstChase = false;
            firstChaseEnemy.SetActive(true);
            enemyFollow.SetCanChase(true);

            firstChaseTrigger.enabled = true;
        }
    }
}
