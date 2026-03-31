using UnityEngine;

public class BillboardEffect : MonoBehaviour
{
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;

    private Vector3 originalRotation;

    private void Awake()
    {
        originalRotation = transform.rotation.eulerAngles;
    }

    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform.position, Vector3.up);

        Vector3 rotation = transform.rotation.eulerAngles;
        if (lockX)
            rotation.x = originalRotation.x;
        if (lockY)
            rotation.y = originalRotation.y;
        if (lockZ)
            rotation.z = originalRotation.z;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
