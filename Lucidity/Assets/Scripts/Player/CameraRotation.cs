using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private PlayerInputObserver inputObserver;
    [SerializeField] private Transform body;

    [SerializeField] private float sensitivity;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float verticalLimit;

    private float startingOrientation = 0f;
    private Vector2 rotation = Vector2.zero;
    private Vector2 currentRotation = Vector2.zero;

    private bool controlEnabled = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        startingOrientation = body.rotation.y;
    }

    void Update()
    {
        if(!controlEnabled) return;

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

    public void SetRotation(Quaternion targetRotation)
    {
        Vector3 euler = targetRotation.eulerAngles;

        float pitch = euler.x;
        if (pitch > 180f) pitch -= 360f;

        float yaw = euler.y;
        if (yaw > 180f) yaw -= 360f;

        pitch = Mathf.Clamp(pitch, -verticalLimit, verticalLimit);

        rotation.x = pitch;
        rotation.y = yaw;

        currentRotation = rotation;

        transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);
        body.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);
    }

    public void SetControlEnabled(bool enabled)
    {
        controlEnabled = enabled;
    }

    public void ResetOrientation()
    {
        rotation.x = 0f;
        rotation.y = startingOrientation;
        currentRotation = rotation;
    }

    public void SetSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
    }
}
