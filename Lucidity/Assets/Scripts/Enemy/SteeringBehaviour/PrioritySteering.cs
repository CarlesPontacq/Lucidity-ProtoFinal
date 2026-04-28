using UnityEngine;
using System.Collections.Generic;

public class PrioritySteering : SteeringBehaviour
{
    [Tooltip("Comportamientos en orden de prioridad (el primero es el más prioritario)")]
    public List<SteeringBehaviour> priorityBehaviours;

    [Tooltip("Radio de área de seguimiento para comportamientos como Seek")]
    public float followAreaRadius = 10f;

    [Tooltip("El target principal (por ejemplo, el jugador)")]
    public Transform mainTarget;

    public override Vector3 CalculateForce(Agent3D agent)
    {
        Vector3 totalForce = Vector3.zero;

        foreach (var behaviour in priorityBehaviours)
        {
            if (behaviour != null)
            {
                Vector3 force = behaviour.CalculateForce(agent);

                if (force.magnitude > 0.1f)
                {
                    totalForce = force;
                    break;
                }
            }
        }

        return totalForce;
    }

    public bool IsTargetInArea(Agent3D agent)
    {
        if (mainTarget == null) return false;

        float distance = Vector3.Distance(agent.Position, mainTarget.position);
        return distance <= followAreaRadius;
    }

    public Transform GetTarget()
    {
        mainTarget = GameManager.PlayerRef.transform;
        return mainTarget;
    }

    public void SetTarget(Transform newTarget)
    {
        mainTarget = newTarget;
    }
}