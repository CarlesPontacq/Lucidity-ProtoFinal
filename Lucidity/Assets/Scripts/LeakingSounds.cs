using System.Collections;
using UnityEngine;

public class LeakingSounds : MonoBehaviour
{
    [SerializeField] private float soundHeight;
    [SerializeField] private float volume = 1f;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float minInterval;
    [SerializeField] private float maxInterval;
    [SerializeField] private bool looping;

    private void Start()
    {
        StartCoroutine(LeakRoutine());
    }

    private IEnumerator LeakRoutine()
    {
        while (looping)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            PlayLeakingSound();
        }
    }

    private void PlayLeakingSound()
    {
        Vector2 randomDir2D = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minDistance, maxDistance);

        Vector3 position = transform.position + new Vector3(
            randomDir2D.x * distance,
            0f,
            randomDir2D.y * distance
        );

        position.y = soundHeight;

        SFXManager.Instance.PlaySpatialSound("leaking", position, volume);
    }

}
