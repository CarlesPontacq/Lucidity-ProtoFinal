using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float stopDistance = 1.2f;

    private Transform target;

    private void Start()
    {
        if (GameManager.PlayerRef != null)
            target = GameManager.PlayerRef.transform;
    }

    private void Update()
    {
        if (target == null) return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f; 

        float dist = toTarget.magnitude;
        if (dist <= stopDistance) return;

        Vector3 dir = toTarget / dist;
        transform.position += dir * moveSpeed * Time.deltaTime;

        //mirar hacia el jugador
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}
