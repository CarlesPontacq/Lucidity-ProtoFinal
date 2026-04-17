using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbodyRef;
    [SerializeField] private Transform bodyRef;
    [SerializeField] private PlayerInputObserver inputObserver;
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;

    private float minMovementMagnitude = 0.0001f;
    private float colliderAdjustment = 0.05f;

    [Header("Wall Slide")]
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float wallCheckDistance = 0.25f;
    [SerializeField] private float wallCheckRadiusMultiplier = 0.9f;

    [Header("Step Handling")]
    [SerializeField] private float maxStepHeight = 0.35f;
    [SerializeField] private float stepCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundMask;
    private string objectToStep = "Stairs";

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

        if (realMovementDir.sqrMagnitude > minMovementMagnitude)
            realMovementDir = realMovementDir.normalized;

        Vector3 finalMovementDir = TryStepUp(realMovementDir);

        if(finalMovementDir == realMovementDir)
            finalMovementDir = GetSlideAdjustedDirection(realMovementDir);

        float speed = inputObserver.IsPressingRun ? runningSpeed : walkingSpeed;

        Vector3 velocity;
        velocity.x = finalMovementDir.x * speed;
        velocity.y = rigidbodyRef.linearVelocity.y;
        velocity.z = finalMovementDir.z * speed;

        rigidbodyRef.linearVelocity = velocity;
    }

    private Vector3 TryStepUp(Vector3 desiredDir)
    {
        if(desiredDir.sqrMagnitude <= minMovementMagnitude) return desiredDir;

        Vector3 capsuleBottom = transform.position + Vector3.up * playerCollider.radius;
        Vector3 capsuleTop = transform.position + Vector3.up * (playerCollider.height - playerCollider.radius);

        bool colision = Physics.CapsuleCast(capsuleBottom, capsuleTop, playerCollider.radius, desiredDir, out RaycastHit hit, stepCheckDistance, obstacleMask);

        if (!colision || hit.transform.name != objectToStep)
            return desiredDir;

        float obstacleHeight = hit.point.y - transform.position.y;

        if (obstacleHeight > maxStepHeight || obstacleHeight < colliderAdjustment)
            return desiredDir;

        Vector3 stepUpPosition = transform.position + Vector3.up * maxStepHeight;
        if (Physics.CheckSphere(stepUpPosition + desiredDir * stepCheckDistance,
            playerCollider.radius * 0.5f, obstacleMask))
            return desiredDir;

        rigidbodyRef.MovePosition(transform.position + Vector3.up * obstacleHeight);

        return desiredDir;
    }

    private Vector3 GetSlideAdjustedDirection(Vector3 desiredDir)
    {
        if (desiredDir.sqrMagnitude <= minMovementMagnitude)
            return Vector3.zero;

        if (playerCollider == null)
            return desiredDir;

        Vector3 origin = transform.position + Vector3.up * (playerCollider.radius + colliderAdjustment);
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
            Vector3 slideDir = Vector3.ProjectOnPlane(desiredDir, hit.normal);
            slideDir.y = 0f;

            if (slideDir.sqrMagnitude > minMovementMagnitude)
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