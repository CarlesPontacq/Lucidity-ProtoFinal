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

    private GameObject enemy = null;

    void Update()
    {
        if (enemy != null)
        {
            float distance = Vector3.Distance(this.transform.position, enemy.transform.position);
            float weight = baseWeight;
            
            if (distance < minDistance)
            {
                weight = maxWeight;
            }
            else
            {
                float t = Mathf.InverseLerp(minDistance, maxDistance, distance);

                weight = Mathf.Lerp(maxWeight, baseWeight, t);
            }

            volume.weight = weight;
        }
    }

    public void SetEnemy(GameObject enemyObj)
    {
        enemy = enemyObj;

        if (enemy == null) volume.weight = 0f;
    }
}