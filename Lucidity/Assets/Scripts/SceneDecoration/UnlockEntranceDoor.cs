using UnityEngine;

public class UnlockEntranceDoor : MonoBehaviour
{
    [SerializeField] private DoorController door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (door != null)
                door.Unlock();
        }
    }
}
