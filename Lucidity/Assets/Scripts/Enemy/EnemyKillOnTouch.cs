using UnityEngine;

public class EnemyKillOnTouch : MonoBehaviour
{
    public bool triggered = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            GameManager.Instance.PlayerDied();
        }
    }

}
