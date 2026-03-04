using System.Threading;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbodyRef;
    [SerializeField] private Transform bodyRef;
    [SerializeField] private PlayerInputObserver inputObserver;
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;

    public bool IsMoving { get; private set; }
    public bool IsRunning { get; private set; }

    void FixedUpdate()
    {
        Move();
        UpdatePublicVariables();
    }

    private void Move()
    {
        Vector2 inputMovementDir = inputObserver.movement.normalized;
        Vector3 realMovementDir = bodyRef.right * inputMovementDir.x + bodyRef.forward * inputMovementDir.y;

        float speed = walkingSpeed;
        if (inputObserver.IsPressingRun)
            speed = runningSpeed;

        rigidbodyRef.linearVelocity = realMovementDir * speed;
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
}
