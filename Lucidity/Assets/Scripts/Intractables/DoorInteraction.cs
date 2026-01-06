using System;
using UnityEngine;

public class DoorInteraction : ObjectInteraction
{
    [SerializeField] Transform pivot;

    [Header("Door Rotation")]
    public float openAngle = 90f;
    public float openSpeed = 5f;

    private bool isOpen = false;
    private bool hasToApplyRotation = false;
    private Quaternion closedRotation;
    private Quaternion targetRotation;

    Vector3 soundPosition;

    protected override void Start()
    {
        base.Start();

        closedRotation = pivot.rotation;
        targetRotation = closedRotation;

        soundPosition = transform.position;
    }

    protected override void Update()
    {
        base.Update();

        ApplyRotation();
    }

    public override void Interact()
    {
        ToogleDoor();
    }

    //Sets up rotation and direction to apply it later
    void ToogleDoor()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            Vector3 doorToPlayer = (GameManager.PlayerRef.transform.position - pivot.position).normalized;

            float side = Vector3.Cross(pivot.right, doorToPlayer).y;
            float direction = side > 0 ? -1f : 1f;

            targetRotation = closedRotation * Quaternion.Euler(0, openAngle * direction, 0);
            SFXManager.Instance.PlaySpatialSound("openDoor", soundPosition, 1f);
        }
        else
        {
            targetRotation = closedRotation;
            SFXManager.Instance.PlaySpatialSound("closeDoor", soundPosition, 1f);
        }

        hasToApplyRotation = true;
    }

    //Checks if it has to apply rotation and of it has to, then it LERPs to that rotation
    void ApplyRotation()
    {
        if (hasToApplyRotation)
        {
            pivot.localRotation = Quaternion.Lerp(pivot.localRotation, targetRotation, Time.deltaTime * openSpeed);

            if (pivot.rotation == targetRotation)
            {
                hasToApplyRotation = false;
            }
        }
    }
}