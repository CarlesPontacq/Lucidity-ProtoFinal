using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyFollowSteering : MonoBehaviour
{
    [Header("Chase")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 1.2f;

    [Header("Turning")]
    [Tooltip("Grados por segundo (más bajo = menos temblor)")]
    [SerializeField] private float turnRateDeg = 180f;

    [Header("Obstacle Avoidance")]
    [SerializeField] private float avoidDistance = 2.0f;
    [SerializeField] private float avoidRadius = 0.35f;
    [SerializeField] private float avoidStrength = 1.0f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Smoothing")]
    [SerializeField] private float directionSmooth = 8f;

    private Rigidbody rb;
    private Transform player;

    private Vector3 smoothMoveDir = Vector3.forward;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Start()
    {
        if (GameManager.PlayerRef != null)
            player = GameManager.PlayerRef.transform;

        smoothMoveDir = transform.forward;
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            if (GameManager.PlayerRef != null) player = GameManager.PlayerRef.transform;
            return;
        }

        Vector3 toPlayer = player.position - rb.position;
        toPlayer.y = 0f;

        float dist = toPlayer.magnitude;
        if (dist <= stopDistance)
            return;

        Vector3 desired = toPlayer.normalized;

        Vector3 avoid = ComputeAvoidance(desired);

        Vector3 rawDir = (desired + avoid).normalized;

        smoothMoveDir = Vector3.Slerp(smoothMoveDir, rawDir, directionSmooth * Time.fixedDeltaTime);
        smoothMoveDir.y = 0f;
        if (smoothMoveDir.sqrMagnitude < 0.0001f) smoothMoveDir = transform.forward;

        Quaternion targetRot = Quaternion.LookRotation(smoothMoveDir, Vector3.up);
        Quaternion newRot = Quaternion.RotateTowards(rb.rotation, targetRot, turnRateDeg * Time.fixedDeltaTime);
        rb.MoveRotation(newRot);

        Vector3 nextPos = rb.position + smoothMoveDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPos);
    }

    private Vector3 ComputeAvoidance(Vector3 desiredDir)
    {
        Vector3 origin = rb.position + Vector3.up * 0.5f;

        if (Physics.SphereCast(origin, avoidRadius, desiredDir, out RaycastHit hit, avoidDistance, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            Vector3 away = hit.normal * avoidStrength;
            away.y = 0f;

            return away;
        }

        return Vector3.zero;
    }
}