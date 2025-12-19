using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private PlayerInputObserver inputObserver;
    [SerializeField] private Transform headRef;
    [SerializeField] private Transform cameraRef;
    private Vector3 startingLocalPositionCamera;


    [Header("HEAD BOB SETTINGS")]
    [SerializeField] private bool headBob;
    [SerializeField] private float amount;
    [SerializeField] private Vector2 amountMultiplier;
    [SerializeField] private float frequency;
    [SerializeField] private Vector2 frequencyMultiplier;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float stoppingSpeed;

    private void Start()
    {
        startingLocalPositionCamera = cameraRef.localPosition;
    }

    void Update()
    {
        transform.position = headRef.position;

        if (headBob)
        {
            if (inputObserver.movement.magnitude > 0)
                StartHeadBob();
            StopHeadBob();
        }
    }

    private void StartHeadBob()
    {
        Vector3 pos = Vector3.zero;

        pos.y = Mathf.Lerp(pos.y, Mathf.Sin(Time.time * frequency * frequencyMultiplier.y) * amount * amountMultiplier.y, smoothSpeed * Time.deltaTime);
        pos.x = Mathf.Lerp(pos.x, Mathf.Sin(Time.time * frequency * frequencyMultiplier.x) * amount * amountMultiplier.x, smoothSpeed * Time.deltaTime);

        Vector3 headBobForward = cameraRef.forward;
        headBobForward.y = 0f;
        headBobForward.Normalize();

        Vector3 headBobRight = Vector3.Cross(Vector3.up, cameraRef.forward);
        Vector3 headBobUp = Vector3.up;

        Vector3 correctedPos = headBobUp * pos.y + headBobRight * pos.x;
        
        cameraRef.localPosition += correctedPos;
    }

    private void StopHeadBob()
    {
        if (cameraRef.localPosition == startingLocalPositionCamera) return;

        cameraRef.localPosition = Vector3.Lerp(cameraRef.localPosition, startingLocalPositionCamera, stoppingSpeed *  Time.deltaTime);
    }
}
