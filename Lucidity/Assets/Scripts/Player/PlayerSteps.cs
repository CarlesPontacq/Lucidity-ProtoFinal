using UnityEngine;
using UnityEngine.Audio;

public class PlayerSteps : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRef;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioResource walkingSound;
    [SerializeField] private AudioResource runningSound;

    const float MinVelocity = 0.1f;

    private bool isPlayingFootsteps;

    void Update()
    {
        Vector3 horizontalVelocity = playerRef.linearVelocity;
        horizontalVelocity.y = 0f;

        float speed = horizontalVelocity.magnitude;

        if (!isPlayingFootsteps && speed > MinVelocity)
        {
            audioSource.Play();
            isPlayingFootsteps = true;
        }
        else if (isPlayingFootsteps && speed <= MinVelocity)
        {
            audioSource.Stop();
            isPlayingFootsteps = false;
        }
    }
}
