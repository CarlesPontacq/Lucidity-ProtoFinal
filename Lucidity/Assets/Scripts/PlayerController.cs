using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbodyRef;
    [SerializeField] private PlayerInputObserver inputObserver;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraSensitivity = 100f;
    [SerializeField] private float movementSpeed = 10f;

    private float xRotation = 0f;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        RotateCamera();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void RotateCamera()
    {
        Vector2 adaptedCameraMovement = inputObserver.cameraMovement * cameraSensitivity;

        xRotation -= inputObserver.cameraMovement.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * inputObserver.cameraMovement.x);
    }

    private void Move()
    {
        Vector2 inputMovementDir = inputObserver.movement.normalized;
        Vector3 realMovementDir = transform.right * inputMovementDir.x + transform.forward * inputMovementDir.y;
        
        rigidbodyRef.linearVelocity = realMovementDir * movementSpeed;

    }
}
