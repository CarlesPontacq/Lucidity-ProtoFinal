using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] Camera playerCameraRef;
    [SerializeField] PlayerInputObserver playerInput;
    [SerializeField] float interactionDistance = 2.5f;
    [SerializeField] float interactionRadius = 0.5f;

    private ObjectInteraction currentInteractionTarget = null;

    private void Start()
    {
        playerInput.onInteract += TryToInteract;  
    }

    private void Update()
    {
        CheckInteraction();
    }

    void CheckInteraction()
    {
        Vector3 origin = playerCameraRef.transform.position;
        Vector3 cameraForward = playerCameraRef.transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(origin, interactionRadius, cameraForward, interactionDistance);

        ObjectInteraction bestTarget = null;
        float bestDot = -1f;
        float bestDistance = float.MaxValue;

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.CompareTag("Interactable"))
                continue;

            Collider collider = hit.collider;

            Vector3 targetPoint = collider.ClosestPoint(origin);
            Vector3 toTarget = targetPoint - origin;
            float distance = toTarget.magnitude;

            if (distance > interactionDistance)
                continue;

            Vector3 dirToTarget = toTarget.normalized;
            if (Physics.Raycast(origin, dirToTarget, out RaycastHit lineHit, distance))
            {
                if (lineHit.collider != collider)
                    continue;
            }

            float dot = Vector3.Dot(cameraForward, dirToTarget);
            if (dot > bestDot || (Mathf.Approximately(dot, bestDot) && distance < bestDistance))
            {
                bestDot = dot;
                bestDistance = distance;
                bestTarget = collider.GetComponent<ObjectInteraction>();
            }
        }

        SetCurrentInteractionTarget(bestTarget);
    }

    void SetCurrentInteractionTarget(ObjectInteraction target)
    {
        if (currentInteractionTarget == target) return;

        if (currentInteractionTarget != null)
        {
            currentInteractionTarget.OnFocusExit();
        }

        currentInteractionTarget = target;

        if (currentInteractionTarget != null)
        {
            currentInteractionTarget.OnFocusEnter();
        }
    }

    void TryToInteract()
    {
        if (currentInteractionTarget != null)
        {
            currentInteractionTarget.Interact();
        }
    }

}
