using UnityEngine;

public class PlayerDeathOnEnemyTouch : MonoBehaviour
{
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private bool useTrigger = true;   
    [SerializeField] private bool useControllerHit = true; 

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return;
        TryKill(other);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!useControllerHit) return;
        if (hit == null || hit.collider == null) return;
        TryKill(hit.collider);
    }

    private void TryKill(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag(enemyTag)) return;

        triggered = true;
        Debug.Log("[Player] Touched enemy -> PlayerDied()");
        GameManager.Instance?.PlayerDied();
    }
}
