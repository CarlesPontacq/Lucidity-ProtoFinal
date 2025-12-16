using System;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public Transform doorPivot;
    public Transform player;

    public float interactionDistance = 2.5f;
    public float lookDotThreshold = 0.5f;
    public float openAngle = 90f;
    public float openSpeed = 5f;

    private bool isOpen = false;
    private bool hasToApplyRotation = false;
    private Quaternion closedRotation;
    private Quaternion targetRotation;

    void Start()
    {
        closedRotation = doorPivot.rotation;
        targetRotation = closedRotation;
    }

    void Update()
    {
        //---------Need to adapt this to new input system
        if(Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }

        ApplyRotation();
    }

    //Checks if the player can interact with the door
    void TryInteract()
    {
        if (!IsPlayerClose()) return;
        if (!IsPlayerLooking()) return;

        ToogleDoor();
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

    //Sets rotation and direction to apply it later
    void ToogleDoor()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            Vector3 doorToPlayer = (player.position - doorPivot.position).normalized;

            float side = Vector3.Cross(doorPivot.right, doorToPlayer).y;
            float direction = side > 0 ? -1f : 1f;

            targetRotation = closedRotation * Quaternion.Euler(0, openAngle * direction, 0);
        }
        else
        {
            targetRotation = closedRotation;
        }

        hasToApplyRotation = true;
    }

    //Checks if it has to apply rotation and of it has to, then it LERPs to that rotation
    void ApplyRotation()
    {
        if (hasToApplyRotation)
        {
            doorPivot.localRotation = Quaternion.Lerp(doorPivot.localRotation, targetRotation, Time.deltaTime * openSpeed);

            if (doorPivot.rotation == targetRotation)
            {
                hasToApplyRotation = false;
            }
        }
    }
}