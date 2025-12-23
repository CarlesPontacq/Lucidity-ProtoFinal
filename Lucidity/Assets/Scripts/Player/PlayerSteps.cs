using UnityEngine;

public class PlayerSteps : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Rigidbody playerRef;

    [SerializeField] private float stepDistance = 2f;

    const float MinVelocity = 0.1f;

    private float distanceAccumulator;

    void Update()
    {
        Vector3 horizontalVelocity = playerRef.linearVelocity;
        horizontalVelocity.y = 0f;

        float speed = horizontalVelocity.magnitude;

        if (speed < MinVelocity)
        {
            distanceAccumulator = 0f;
            return;
        }

        distanceAccumulator += speed * Time.deltaTime;

        if (distanceAccumulator >= stepDistance)
        {
            audioSource.Play();
            distanceAccumulator = 0f;
        }
    }
}
