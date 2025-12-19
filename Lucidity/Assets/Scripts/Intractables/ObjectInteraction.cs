using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    [Header("Base Object Interaction")]
    public Transform objectToInteractWith;
    public Transform player;

    public float interactionDistance = 2.5f;
    public float lookDotThreshold = 0.5f;

    protected bool canInteract = false;
    protected bool hasToApplyInteraction = false;

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        //---------Need to adapt this to new input system
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryToInteract();
        }
    }

    //Checks if the player can interact with the door
    protected virtual void TryToInteract()
    {
        canInteract = false;

        if (!IsPlayerClose()) return;
        if (!IsPlayerLooking()) return;

        canInteract = true;
    }

    //Checks if the player is near the door
    bool IsPlayerClose()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        return distance <= interactionDistance;
    }

    //Checks if the player is looking in the general direction of the door
    bool IsPlayerLooking()
    {
        Vector3 dirToDoor = (transform.position - player.position).normalized;
        float dot = Vector3.Dot(player.forward, dirToDoor);

        return dot >= lookDotThreshold;
    }
}
