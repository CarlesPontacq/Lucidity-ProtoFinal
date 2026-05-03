using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct BehaviourAndWeight
{
    public SteeringBehaviour behaviour;
    [Range(0f, 5f)]
    public float weight;
}

public class BlendedSteering : SteeringBehaviour
{
    [SerializeField] private List<BehaviourAndWeight> behaviours;

    public override Vector3 CalculateForce(Agent agent)
    {
        Vector3 totalSteeringForce = Vector3.zero;

        foreach (var entry in behaviours) 
        { 
            if(entry.behaviour != null && entry.weight > 0)
            {
                totalSteeringForce += entry.behaviour.CalculateForce(agent) * entry.weight;
            }
        }

        return totalSteeringForce;
    }
}
