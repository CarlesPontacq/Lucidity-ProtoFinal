using UnityEngine;

public class PlayerLooper : MonoBehaviour
{
    [Header("Normal loop")]
    [SerializeField] private Transform teleportDestination;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private GameObject playerBodyRef;
    [SerializeField] private CameraRotation playerCameraRotationRef;
    [SerializeField] private CameraRotation cameraCameraRotationRef;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (GameManager.Instance.finishedLoops)
            {
                Debug.Log("Finished Game");
                return;
            }

            SetPlayerLoopPositionPosition();
        }
    }

    private void SetPlayerLoopPositionPosition()
    {
        Vector3 localOffset = transform.InverseTransformPoint(playerRb.transform.position);
        Quaternion relativeRotation = teleportDestination.rotation * Quaternion.Inverse(transform.rotation);

        playerRb.transform.position = teleportDestination.TransformPoint(localOffset);
        playerCameraRotationRef.ApplyRotationOffset(relativeRotation);
        cameraCameraRotationRef.ApplyRotationOffset(relativeRotation);

        GameManager.Instance.OnExitDoorCrossed();
    }
}
