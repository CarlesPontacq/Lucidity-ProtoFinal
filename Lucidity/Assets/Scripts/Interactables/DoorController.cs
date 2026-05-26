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

    [Header("SFX")]
    [SerializeField] private string openDoorSFX = "openDoor";
    [SerializeField] private string closeDoorSFX = "closeDoor";
    public bool IsInteractable => !isLocked;

    private bool isOpen = false;
    private bool isLocked = false;

    private bool hasToApplyRotation = false;
    private bool hasStarted = false;

    private Vector3 soundPosition;

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

        TrySwitchPivot();
    }

    private void TrySwitchPivot()
    {
        float currentY = currentPivot.localRotation.eulerAngles.y;
        float closedY = leftClosedRotation.eulerAngles.y;

        float delta = Mathf.DeltaAngle(currentY, closedY);

        if (pendingPivotChange != 0f)
        {
            bool crossedZero = Mathf.Sign(previousDelta) != Mathf.Sign(delta);

            if (crossedZero)
            {
                SetPivot(pendingPivotChange);
            }
        }

        previousDelta = delta;
    }

    private float GetOpenDirection()
    {
        if (GameManager.PlayerRef == null)
            return Mathf.Sign(defaultOpenDirection);

        float leftDistance = Vector3.Distance(GameManager.PlayerRef.transform.position, pivotLeft.position);
        float rightDistance = Vector3.Distance(GameManager.PlayerRef.transform.position, pivotRight.position);

        return leftDistance < rightDistance ? -1f : 1f;
    }

    private void SetPivot(float direction)
    {
        Transform newPivot = direction == 1f ? pivotLeft : pivotRight;

        if (currentPivot == newPivot) return;

        currentPivot = newPivot;

        transform.SetParent(currentPivot, true);

        ApplyCorrectLocalOffset();

        pendingPivotChange = 0f;
    }

    private void ApplyCorrectLocalOffset()
    {
        if (currentPivot == pivotLeft)
        {
            transform.localPosition = leftLocalPos;
            transform.localRotation = leftLocalRot;
        }
        else
        {
            transform.localPosition = rightLocalPos;
            transform.localRotation = rightLocalRot;
        }
    }

    public void Toggle()
    {
        if (isLocked) return;

        if (isOpen)
            Close(true);
        else
            Open(true);
    }

    public void Open(bool animate)
    {
        if (isOpen) return;

        isOpen = true;

        float direction = GetOpenDirection();

        if (hasToApplyRotation)
            pendingPivotChange = direction;
        else
            SetPivot(direction);

        targetRotation = direction == 1f ? leftOpenRotation : rightOpenRotation;

        if (animate)
        {
            hasToApplyRotation = true;

            SFXManager.Instance.PlaySpatialSound(openDoorSFX, soundPosition, 1f);
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

            SFXManager.Instance.PlaySpatialSound(closeDoorSFX, soundPosition, 1f);
        }
        else
        {
            hasToApplyRotation = false;
            currentPivot.localRotation = targetRotation;
        }
    }

    public void ResetToInitialState(bool animate)
    {
        if (startsOpen && !isOpen)
            Open(animate);
        else if (!startsOpen && isOpen)
            Close(animate);

        if (startsLocked && !isLocked)
            Lock();
        else if (!startsLocked && isLocked)
            Unlock();
    }

    public void Unlock()
    {
        bool wasLocked = isLocked;

        isLocked = false;

        if (hasStarted && wasLocked && autoOpenWhenUnlocked)
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
}