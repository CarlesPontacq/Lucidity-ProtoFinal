using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private PlayerInputObserver inputObserver;
    [SerializeField] private Transform body;

    [SerializeField] private float sensitivity;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float verticalLimit;

    private Vector2 rotation = Vector2.zero;
    private Vector2 currentRotation = Vector2.zero;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 adaptedCameraMovement = inputObserver.cameraMovement * sensitivity;

        rotation.y += adaptedCameraMovement.x;
        rotation.x -= adaptedCameraMovement.y;

        rotation.x = Mathf.Clamp(rotation.x, -verticalLimit, verticalLimit);

        currentRotation.x = Mathf.Lerp(currentRotation.x, rotation.x, smoothSpeed * Time.deltaTime);
        currentRotation.y = Mathf.Lerp(currentRotation.y, rotation.y, smoothSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);
        body.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);
    }

    public void ApplyRotationOffset(Quaternion rotationOffset)
    {
        Vector3 eulerOffset = rotationOffset.eulerAngles;

        rotation.y += eulerOffset.y;
        currentRotation.y += eulerOffset.y;

        rotation.x += eulerOffset.x;
        currentRotation.x += eulerOffset.x;
    }
}
