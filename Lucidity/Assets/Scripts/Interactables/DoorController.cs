using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Pivots")]
    [SerializeField] private Transform pivotLeft;
    [SerializeField] private Transform pivotRight;

    [Header("State")]
    [SerializeField] private bool startsOpen;
    [SerializeField] private bool startsLocked;

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

    public bool IsInteractable => !isLocked;

    private bool isOpen;
    private bool isLocked;

    private bool hasToApplyRotation;
    private bool hasStarted;

    private Transform currentPivot;

    private Quaternion leftClosedRotation;
    private Quaternion rightClosedRotation;
    private Quaternion leftOpenRotation;
    private Quaternion rightOpenRotation;

    private Vector3 leftLocalPos;
    private Quaternion leftLocalRot;
    private Vector3 rightLocalPos;
    private Quaternion rightLocalRot;

    private Quaternion targetRotation;

    private float pendingPivotChange;
    private float previousDelta;

    private Vector3 soundPosition;

    private void Start()
    {
        leftLocalPos = pivotLeft.InverseTransformPoint(transform.position);
        leftLocalRot = Quaternion.Inverse(pivotLeft.rotation) * transform.rotation;

        rightLocalPos = pivotRight.InverseTransformPoint(transform.position);
        rightLocalRot = Quaternion.Inverse(pivotRight.rotation) * transform.rotation;

        currentPivot = pivotLeft;

        leftClosedRotation = pivotLeft.localRotation;
        rightClosedRotation = pivotRight.localRotation;

        leftOpenRotation = leftClosedRotation * Quaternion.Euler(0f, openAngle, 0f);
        rightOpenRotation = rightClosedRotation * Quaternion.Euler(0f, -openAngle, 0f);

        soundPosition = transform.position;
        hasStarted = true;

        if (startsOpen)
            Open(false);
        else
            Close(false);

        if (startsLocked)
            Lock();
    }

    private void Update()
    {
        ApplyRotation();
    }

    private void ApplyRotation()
    {
        if (!hasToApplyRotation) return;

        currentPivot.localRotation =
            Quaternion.Lerp(currentPivot.localRotation, targetRotation, Time.deltaTime * openSpeed);

        if (Quaternion.Angle(currentPivot.localRotation, targetRotation) < 0.1f)
        {
            currentPivot.localRotation = targetRotation;
            hasToApplyRotation = false;
        }
    }

    public void Toggle()
    {
        if (isLocked) return;

        if (isOpen) Close(true);
        else Open(true);
    }

    public void Open(bool animate)
    {
        if (isOpen) return;

        isOpen = true;

        float direction = GetOpenDirection();

        targetRotation = direction == 1f ? leftOpenRotation : rightOpenRotation;

        if (animate)
        {
            hasToApplyRotation = true;
            SFXManager.Instance.PlaySpatialSound("openDoor", soundPosition, 1f);
        }
        else
        {
            hasToApplyRotation = false;
            currentPivot.localRotation = targetRotation;
        }
    }

    public void Close(bool animate)
    {
        if (!isOpen) return;

        isOpen = false;

        targetRotation = currentPivot == pivotLeft ? leftClosedRotation : rightClosedRotation;

        if (animate)
        {
            hasToApplyRotation = true;
            SFXManager.Instance.PlaySpatialSound("closeDoor", soundPosition, 1f);
        }
        else
        {
            hasToApplyRotation = false;
            currentPivot.localRotation = targetRotation;
        }
    }

    private float GetOpenDirection()
    {
        if (GameManager.PlayerRef == null)
            return Mathf.Sign(defaultOpenDirection);

        float left = Vector3.Distance(GameManager.PlayerRef.transform.position, pivotLeft.position);
        float right = Vector3.Distance(GameManager.PlayerRef.transform.position, pivotRight.position);

        return left < right ? -1f : 1f;
    }

    public void Unlock()
    {
        isLocked = false;

        if (autoOpenWhenUnlocked)
            Open(autoOpenAnimated);
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

    public void CopyFromOld(DoorInteraction old)
    {
        startsOpen = old.StartsOpen;
        startsLocked = old.StartsLocked;

        autoOpenWhenUnlocked = old.AutoOpenWhenUnlocked;
        autoOpenAnimated = old.AutoOpenAnimated;
        defaultOpenDirection = old.DefaultOpenDirection;
        requiresReportToOpen = old.RequiresReportToOpen;

        openAngle = old.OpenAngle;
        openSpeed = old.OpenSpeed;
    }
}