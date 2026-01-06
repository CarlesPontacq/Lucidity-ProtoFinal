using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] Camera playerCameraRef;
    [SerializeField] PlayerInputObserver playerInput;
    [SerializeField] float interactionDistance = 2.5f;
    [SerializeField] float interactionRadius = 0.5f;

    ObjectInteraction currentInteractionTarget = null;

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
        Debug.DrawRay(playerCameraRef.transform.position, playerCameraRef.transform.forward * interactionDistance, Color.red);

        ObjectInteraction newTarget = null;

        Ray ray = new Ray(playerCameraRef.transform.position, playerCameraRef.transform.forward);

        if (Physics.SphereCast(ray, interactionRadius, out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                newTarget = hit.collider.gameObject.GetComponent<ObjectInteraction>();
            }
        }

        SetCurrentInteractionTarget(newTarget);
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
