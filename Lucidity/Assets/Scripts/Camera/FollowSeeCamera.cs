using UnityEngine;

public class FollowSeeCamera : MonoBehaviour
{
    public Transform playerSeeCamera;

    void Start()
    {
        gameObject.transform.position = playerSeeCamera.position;
        gameObject.transform.rotation = playerSeeCamera.rotation;
    }


    void Update()
    {
        gameObject.transform.position = playerSeeCamera.position;
        gameObject.transform.rotation = playerSeeCamera.rotation;
    }
}
