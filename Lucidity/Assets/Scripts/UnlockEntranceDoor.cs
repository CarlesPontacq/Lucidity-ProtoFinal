using UnityEngine;

public class UnlockEntranceDoor : MonoBehaviour
{
    [SerializeField] private DoorInteraction door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (door != null)
                door.Unlock();
        }
    }
}
