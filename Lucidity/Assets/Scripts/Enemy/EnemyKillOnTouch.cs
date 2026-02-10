using UnityEngine;

public class EnemyKillOnTouch : MonoBehaviour
{
    public bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        GameManager.Instance?.PlayerDied();
    }
}
