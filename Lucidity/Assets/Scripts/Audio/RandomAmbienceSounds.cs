using System.Collections;
using UnityEngine;

public class RandomAmbienceSounds : MonoBehaviour
{
    [SerializeField] private float soundHeight;
    [SerializeField] private float volume = 1f;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float minInterval;
    [SerializeField] private float maxInterval;
    [SerializeField] private bool looping;

    [SerializeField] private string sfx = "leaking";

    private void Start()
    {
        StartCoroutine(RandomAmbienceRoutine());
    }

    private IEnumerator RandomAmbienceRoutine()
    {
        while (looping)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            PlayRandomAmbienceSound();
        }
    }

    private void PlayRandomAmbienceSound()
    {
        Vector2 randomDir2D = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minDistance, maxDistance);

        Vector3 position = transform.position + new Vector3(
            randomDir2D.x * distance,
            0f,
            randomDir2D.y * distance
        );

        position.y = soundHeight;

        SFXManager.Instance.PlaySpatialSound(sfx, position, volume);
    }

}
