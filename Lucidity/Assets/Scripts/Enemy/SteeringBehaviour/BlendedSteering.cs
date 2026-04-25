using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct BehaviourAndWeight3D
{
    public SteeringBehaviour behaviour;
    [Range(0f, 5f)]
    public float weight;
}

public class BlendedSteering: SteeringBehaviour
{
    [Tooltip("Llista de comportaments i els seus pesos per a la combinació.")]
    public List<BehaviourAndWeight3D> behaviours;

    public override Vector3 CalculateForce(Agent3D agent)
    {
        Vector3 totalSteeringForce = Vector3.zero;

        foreach (var entry in behaviours)
        {
            if (entry.behaviour != null && entry.weight > 0)
            {
                totalSteeringForce += entry.behaviour.CalculateForce(agent) * entry.weight;
            }
        }

        return Vector3.ClampMagnitude(totalSteeringForce, agent.maxForce);
    }
}