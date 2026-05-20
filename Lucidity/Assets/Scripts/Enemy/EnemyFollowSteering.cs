using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyFollowSteering : MonoBehaviour
{
    [Header("Chase")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 1.2f;

    [Header("Turning")]
    [SerializeField] private float turnRateDeg = 180f;

    [Header("Obstacle Avoidance")]
    [SerializeField] private float avoidDistance = 4.0f;
    [SerializeField] private float avoidRadius = 0.35f;
    [SerializeField] private float avoidStrength = 1.0f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Smoothing")]
    [SerializeField] private float directionSmooth = 8f;

    private Rigidbody rb;
    private Transform player;
    private Vector3 smoothMoveDir = Vector3.forward;

    private bool canChase = false;

    public void SetCanChase(bool value)
    {
        canChase = value;
    }

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
        if (!canChase) return;

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

        if (smoothMoveDir.sqrMagnitude < 0.0001f)
            smoothMoveDir = transform.forward;

        Quaternion targetRot = Quaternion.LookRotation(smoothMoveDir, Vector3.up);
        Quaternion newRot = Quaternion.RotateTowards(rb.rotation, targetRot, turnRateDeg * Time.fixedDeltaTime);
        rb.MoveRotation(newRot);

        Vector3 nextPos = rb.position + smoothMoveDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPos);
    }

    private Vector3 ComputeAvoidance(Vector3 desiredDir)
    {
        Vector3 origin = rb.position + Vector3.up * 0.5f;
        Vector3 avoidanceForce = Vector3.zero;
        int hitCount = 0;

        Vector3[] directions = {
        desiredDir,
        Quaternion.Euler(0, 30, 0) * desiredDir,
        Quaternion.Euler(0, -30, 0) * desiredDir,
        Quaternion.Euler(0, 60, 0) * desiredDir,
        Quaternion.Euler(0, -60, 0) * desiredDir,
        Quaternion.Euler(0, 90, 0) * desiredDir,
        Quaternion.Euler(0, -90, 0) * desiredDir
    };

        foreach (Vector3 dir in directions)
        {
            if (Physics.SphereCast(origin, avoidRadius, dir, out RaycastHit hit, avoidDistance, obstacleMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 awayFromObstacle;

                if (hit.distance < avoidRadius * 1.5f)
                {
                    awayFromObstacle = -hit.normal;
                }
                else
                {
                    Vector3 rightTurn = Vector3.Cross(Vector3.up, hit.normal).normalized;
                    Vector3 leftTurn = -rightTurn;

                    float rightDot = Vector3.Dot(rightTurn, desiredDir);
                    float leftDot = Vector3.Dot(leftTurn, desiredDir);

                    awayFromObstacle = (rightDot > leftDot) ? rightTurn : leftTurn;

                    awayFromObstacle = (awayFromObstacle + (-hit.normal) * 0.3f).normalized;
                }

                float distanceWeight = 1f - Mathf.Clamp01(hit.distance / avoidDistance);
                float strengthMultiplier = Mathf.Lerp(0.5f, 2f, distanceWeight);

                avoidanceForce += awayFromObstacle * avoidStrength * strengthMultiplier;
                hitCount++;
            }
        }

        if (hitCount > 0)
        {
            avoidanceForce /= hitCount;
            avoidanceForce.y = 0f;

            if (avoidanceForce.sqrMagnitude > 1f)
                avoidanceForce.Normalize();

            Vector3 finalForce = avoidanceForce * avoidStrength;

            if (hitCount >= 3)
                return finalForce.normalized * avoidStrength;

            return finalForce;
        }

        return Vector3.zero;
    }

    public void SetChaseSpeed(float speed)
    {
        moveSpeed = speed;
    }
}