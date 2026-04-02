using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbodyRef;
    [SerializeField] private Transform bodyRef;
    [SerializeField] private PlayerInputObserver inputObserver;
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;

    [Header("Wall Slide")]
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float wallCheckDistance = 0.25f;
    [SerializeField] private float wallCheckRadiusMultiplier = 0.9f;

    public bool IsMoving { get; private set; }
    public bool IsRunning { get; private set; }

    private void FixedUpdate()
    {
        Move();
        UpdatePublicVariables();
    }

    private void Move()
    {
        Vector2 inputMovementDir = inputObserver.movement.normalized;

        Vector3 realMovementDir = bodyRef.right * inputMovementDir.x + bodyRef.forward * inputMovementDir.y;
        realMovementDir.y = 0f;

        if (realMovementDir.sqrMagnitude > 0.0001f)
            realMovementDir = realMovementDir.normalized;

        // Ajuste para deslizar por paredes
        Vector3 finalMovementDir = GetSlideAdjustedDirection(realMovementDir);

        float speed = walkingSpeed;
        if (inputObserver.IsPressingRun)
            speed = runningSpeed;

        Vector3 velocity;
        velocity.x = finalMovementDir.x * speed;
        velocity.y = rigidbodyRef.linearVelocity.y;
        velocity.z = finalMovementDir.z * speed;

        rigidbodyRef.linearVelocity = velocity;
    }

    private Vector3 GetSlideAdjustedDirection(Vector3 desiredDir)
    {
        if (desiredDir.sqrMagnitude <= 0.0001f)
            return Vector3.zero;

        if (playerCollider == null)
            return desiredDir;

        Vector3 origin = transform.position + Vector3.up * (playerCollider.radius + 0.05f);
        float sphereRadius = playerCollider.radius * wallCheckRadiusMultiplier;

        if (Physics.SphereCast(
                origin,
                sphereRadius,
                desiredDir,
                out RaycastHit hit,
                wallCheckDistance,
                obstacleMask,
                QueryTriggerInteraction.Ignore))
        {
            // Proyectar dirección sobre el plano de la pared
            Vector3 slideDir = Vector3.ProjectOnPlane(desiredDir, hit.normal);
            slideDir.y = 0f;

            if (slideDir.sqrMagnitude > 0.0001f)
                return slideDir.normalized;

            return Vector3.zero;
        }

        return desiredDir;
    }

    private void UpdatePublicVariables()
    {
        Vector3 horizontalVelocity = rigidbodyRef.linearVelocity;
        horizontalVelocity.y = 0f;
        float currentSpeed = horizontalVelocity.magnitude;

        IsMoving = currentSpeed > 0.01f;
        IsRunning = IsMoving && currentSpeed > walkingSpeed + 0.01f;
    }

    public float GetWalkingSpeed()
    {
        return walkingSpeed;
    }

    public float GetRunningSpeed()
    {
        return runningSpeed;
    }

    private void OnDisable()
    {
        IsMoving = false;
        IsRunning = false;
    }
}