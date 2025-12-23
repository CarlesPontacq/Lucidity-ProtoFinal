using UnityEngine;
using UnityEngine.Audio;

public class PlayerSteps : MonoBehaviour
{
    private enum StepsType { Walking, Running }

    [SerializeField] private Rigidbody playerRef;
    [SerializeField] private PlayerInputObserver inputObserver;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioResource walkingSound;
    [SerializeField] private AudioResource runningSound;

    private const float MinVelocity = 0.1f;

    private StepsType currentStepsType;

    void Update()
    {
        if (!IsMoving())
        {
            StopFootsteps();
            return;
        }

        StepsType desiredStepsType = inputObserver.IsPressingRun ? StepsType.Running : StepsType.Walking;

        UpdateStepsType(desiredStepsType);
        PlayFootstepsIfNeeded();
    }

    private bool IsMoving()
    {
        Vector3 velocity = playerRef.linearVelocity;
        velocity.y = 0f;
        return velocity.magnitude > MinVelocity;
    }

    private void UpdateStepsType(StepsType newType)
    {
        if (newType == currentStepsType)
            return;

        audioSource.resource = newType == StepsType.Running ? runningSound : walkingSound;

        currentStepsType = newType;

        if (audioSource.isPlaying)
            audioSource.Play();
    }

    private void PlayFootstepsIfNeeded()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    private void StopFootsteps()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }
}
