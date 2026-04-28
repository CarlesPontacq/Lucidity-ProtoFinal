using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerEnemyDetection : MonoBehaviour
{
    [SerializeField] Volume volume;

    [SerializeField] float minDistance;
    [SerializeField] float maxDistance;

    [SerializeField] float baseWeight;
    [SerializeField] float maxWeight;

    [SerializeField] float smoothSpeed = 5f;

    [SerializeField] GameObject enemy = null;
    private float targetWeight = 0f;

    void Update()
    {
        if (enemy != null)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < minDistance)
            {
                targetWeight = maxWeight;
            }
            else
            {
                float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
                targetWeight = Mathf.Lerp(maxWeight, baseWeight, t);
            }
        }
        else
        {
            targetWeight = 0f;
        }

        volume.weight = Mathf.Lerp(volume.weight, targetWeight, Time.deltaTime * smoothSpeed);
    }

    public void SetEnemy(GameObject enemyObj)
    {
        enemy = enemyObj;
    }
}