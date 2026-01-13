using System;
using UnityEngine;

public class DoorInteraction : ObjectInteraction
{
    [SerializeField] Transform pivot;

    [Header("Door Rotation")]
    public float openAngle = 90f;
    public float openSpeed = 5f;

    [Header("Exit Door (optional)")]
    [SerializeField] private bool requiresReportToOpen = false;
    [SerializeField] private ReportResultState reportState;
    [SerializeField] private bool isLocked = true;

    private bool isOpen = false;
    private bool hasToApplyRotation = false;

    private Quaternion closedLocalRotation;
    private Quaternion targetLocalRotation;

    Vector3 soundPosition;

    protected override void Start()
    {
        base.Start();

        closedLocalRotation = pivot.localRotation;
        targetLocalRotation = closedLocalRotation;

        pivot.localRotation = closedLocalRotation;

        soundPosition = transform.position;

        if (!requiresReportToOpen) isLocked = false;
    }

    protected override void Update()
    {
        base.Update();
        ApplyRotation();
    }

    public override void Interact()
    {
        if (requiresReportToOpen && isLocked)
        {
            Debug.Log("Puerta bloqueada: firma el documento primero.");
            return;
        }

        ToggleDoor();
    }

    public void UnlockExitDoor() => isLocked = false;

    public void LockExitDoor()
    {
        if (requiresReportToOpen)
            isLocked = true;
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            Vector3 doorToPlayer = (GameManager.PlayerRef.transform.position - pivot.position).normalized;

            float side = Vector3.Cross(pivot.right, doorToPlayer).y;
            float direction = side > 0 ? -1f : 1f;

            targetLocalRotation = closedLocalRotation * Quaternion.Euler(0f, openAngle * direction, 0f);
            SFXManager.Instance.PlaySpatialSound("openDoor", soundPosition, 1f);
        }
        else
        {
            targetLocalRotation = closedLocalRotation;
            SFXManager.Instance.PlaySpatialSound("closeDoor", soundPosition, 1f);
        }

        hasToApplyRotation = true;
    }

    void ApplyRotation()
    {
        if (!hasToApplyRotation) return;

        pivot.localRotation = Quaternion.Lerp(pivot.localRotation, targetLocalRotation, Time.deltaTime * openSpeed);

        if (Quaternion.Angle(pivot.localRotation, targetLocalRotation) < 0.1f)
        {
            pivot.localRotation = targetLocalRotation;
            hasToApplyRotation = false;
        }
    }
}
