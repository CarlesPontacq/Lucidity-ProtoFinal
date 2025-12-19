using UnityEngine;

public class FollowSeeCamera : MonoBehaviour
{
    public Transform playerSeeCamera;

    void Start()
    {
        gameObject.transform.position = playerSeeCamera.position;
    }


    void Update()
    {
        gameObject.transform.position = playerSeeCamera.position;
    }
}
