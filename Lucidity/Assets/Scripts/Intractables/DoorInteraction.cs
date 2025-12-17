using System;
using UnityEngine;

public class DoorInteraction : ObjectInteraction
{
    [Header("Door Rotation")]
    public float openAngle = 90f;
    public float openSpeed = 5f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion targetRotation;

    protected override void Start()
    {
        base.Start();

        closedRotation = objectToInteractWith.rotation;
        targetRotation = closedRotation;
    }

    protected override void Update()
    {
        base.Update();

        ApplyRotation();
    }

    //Use parent script TryToInteract and add logic to setup door rotation
    protected override void TryToInteract()
    {
        base.TryToInteract();

        if(canInteract) ToogleDoor();
    }

    //Sets up rotation and direction to apply it later
    void ToogleDoor()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            Vector3 doorToPlayer = (player.position - objectToInteractWith.position).normalized;

            float side = Vector3.Cross(objectToInteractWith.right, doorToPlayer).y;
            float direction = side > 0 ? -1f : 1f;

            targetRotation = closedRotation * Quaternion.Euler(0, openAngle * direction, 0);
        }
        else
        {
            targetRotation = closedRotation;
        }

        hasToApplyInteraction = true;
    }

    //Checks if it has to apply rotation and of it has to, then it LERPs to that rotation
    void ApplyRotation()
    {
        if (hasToApplyInteraction)
        {
            objectToInteractWith.localRotation = Quaternion.Lerp(objectToInteractWith.localRotation, targetRotation, Time.deltaTime * openSpeed);

            if (objectToInteractWith.rotation == targetRotation)
            {
                hasToApplyInteraction = false;
            }
        }
    }
}