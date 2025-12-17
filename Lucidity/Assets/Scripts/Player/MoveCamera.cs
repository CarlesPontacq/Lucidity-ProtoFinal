using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform headPosition;

    void Update()
    {
        transform.position = headPosition.position;
    }
}
