using System.Threading;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbodyRef;
    [SerializeField] private Transform bodyRef;
    [SerializeField] private PlayerInputObserver inputObserver;
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;

    void FixedUpdate()
    {
        Move();
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
}
