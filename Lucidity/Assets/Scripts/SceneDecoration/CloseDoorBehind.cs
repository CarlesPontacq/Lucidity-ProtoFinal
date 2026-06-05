using UnityEngine;

public class CloseDoorBehind : MonoBehaviour
{
    [SerializeField] DoorController door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.Close(true);
            door.Lock();
        }
    }
}
