using System;
using UnityEngine;

public class DoorInteraction : ObjectInteraction
{
    [SerializeField] private Transform pivot;
    [SerializeField] private bool isLocked = true;

    [Header("Door Rotation")]
    public float openAngle = 90f;
    public float openSpeed = 5f;

    [Header("Auto Open On Unlock")]
    [SerializeField] private bool autoOpenWhenUnlocked = true;
    [SerializeField] private bool autoOpenAnimated = false;
    [SerializeField] private float defaultOpenDirection = 1f; 

    [Header("Exit Door (optional)")]
    [SerializeField] private bool requiresReportToOpen = false;
    [SerializeField] private ReportResultState reportState;

    private bool isOpen = false;
    private bool hasToApplyRotation = false;
    private bool hasStarted = false;

    private Quaternion closedLocalRotation;
    private Quaternion targetLocalRotation;

    private Vector3 soundPosition;

    protected override void Start()
    {
        base.Start();

        closedLocalRotation = pivot.localRotation;
        targetLocalRotation = closedLocalRotation;

        isOpen = false;
        hasToApplyRotation = false;
        pivot.localRotation = closedLocalRotation;

        soundPosition = transform.position;
        hasStarted = true;
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

        if (!isLocked)
            Toggle();
        else
            Debug.Log("Puerta bloqueada");
    }

    public void Unlock()
    {
        bool wasLocked = isLocked;
        isLocked = false;

        if (hasStarted && wasLocked && autoOpenWhenUnlocked)
        {
            Open(autoOpenAnimated);
        }
    }

    public void Lock()
    {
        isLocked = true;
    }

    public void LockExitDoor()
    {
        if (requiresReportToOpen)
            isLocked = true;

        Close(false);
    }

    public void Open(bool animate)
    {
        if (isOpen) return;

        isOpen = true;

        float direction = GetOpenDirection();
        targetLocalRotation = closedLocalRotation * Quaternion.Euler(0f, openAngle * direction, 0f);

        if (animate)
        {
            hasToApplyRotation = true;
            SFXManager.Instance.PlaySpatialSound("openDoor", soundPosition, 1f);
        }
        else
        {
            hasToApplyRotation = false;
            pivot.localRotation = targetLocalRotation;
        }
    }

    public void Close(bool animate)
    {
        if (!isOpen) return;

        isOpen = false;
        targetLocalRotation = closedLocalRotation;

        if (animate)
        {
            hasToApplyRotation = true;
            SFXManager.Instance.PlaySpatialSound("closeDoor", soundPosition, 1f);
        }
        else
        {
            hasToApplyRotation = false;
            pivot.localRotation = closedLocalRotation;
        }
    }

    private void Toggle()
    {
        if (isOpen)
        {
            Close(true);
        }
        else
        {
            Open(true);
        }
    }

    private float GetOpenDirection()
    {
        if (GameManager.PlayerRef == null)
            return Mathf.Sign(defaultOpenDirection);

        Vector3 doorToPlayer = (GameManager.PlayerRef.transform.position - pivot.position).normalized;
        float side = Vector3.Cross(pivot.right, doorToPlayer).y;

        return side > 0 ? -1f : 1f;
    }

    private void ApplyRotation()
    {
        if (!hasToApplyRotation) return;

        pivot.localRotation = Quaternion.Lerp(
            pivot.localRotation,
            targetLocalRotation,
            Time.deltaTime * openSpeed
        );

        if (Quaternion.Angle(pivot.localRotation, targetLocalRotation) < 0.1f)
        {
            pivot.localRotation = targetLocalRotation;
            hasToApplyRotation = false;
        }
    }
}