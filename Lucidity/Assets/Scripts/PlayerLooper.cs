using UnityEngine;

public class PlayerLooper : MonoBehaviour
{
    [SerializeField] private Transform teleportDestination;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private GameObject playerBodyRef;
    [SerializeField] private CameraRotation playerCameraRotationRef;
    [SerializeField] private CameraRotation cameraCameraRotationRef;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Vector3 localOffset = transform.InverseTransformPoint(playerRb.transform.position);
            Quaternion relativeRotation = teleportDestination.rotation * Quaternion.Inverse(transform.rotation);

            playerRb.transform.position = teleportDestination.TransformPoint(localOffset);
            playerCameraRotationRef.ApplyRotationOffset(relativeRotation);
            cameraCameraRotationRef.ApplyRotationOffset(relativeRotation);

            // Por ahora se suma siempre
            GameManager.Instance.AddLoopToCount();
        }
    }
}
