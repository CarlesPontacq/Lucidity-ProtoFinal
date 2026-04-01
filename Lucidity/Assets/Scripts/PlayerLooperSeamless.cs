using UnityEngine;

public class PlayerLooperSeamless : MonoBehaviour
{
    [Header("Normal loop")]
    [SerializeField] private Transform teleportDestination;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private GameObject playerBodyRef;
    [SerializeField] private CameraRotation playerCameraRotationRef;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (GameManager.Instance.finishedLoops)
                return;

            SetPlayerLoopPositionPosition();
        }
    }

    private void SetPlayerLoopPositionPosition()
    {
        Vector3 localOffset = transform.InverseTransformPoint(playerRb.transform.position);
        Quaternion relativeRotation = teleportDestination.rotation * Quaternion.Inverse(transform.rotation);

        playerRb.transform.position = teleportDestination.TransformPoint(localOffset);
        if (playerCameraRotationRef != null)
            playerCameraRotationRef.ApplyRotationOffset(relativeRotation);

        GameManager.Instance.OnExitDoorCrossed();
    }
}
