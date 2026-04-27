using UnityEngine;

public class Seek : SteeringBehaviour
{
    [Tooltip("La diana (el jugador) que l'agent ha de perseguir.")]
    public Transform target;
    public float stopDistance = 1.2f;
    public float followArea = 0;
    private Vector3 offset = Vector3.zero;

    private void Start()
    {
        offset.y = 0.5f;
        target = GameManager.PlayerRef.transform;
    }

    public override Vector3 CalculateForce(Agent3D agent)
    {
        if (target == null) return Vector3.zero;

        // Verificar si estamos dentro del área de seguimiento (si aplica)
        if (GetComponent<PrioritySteering>() != null)
        {
            if (!GetComponent<PrioritySteering>().IsTargetInArea(agent))
            {
                return Vector3.zero;
            }
        }

        SetTarget();

        Vector3 toTarget = (target.position + offset) - agent.Position;
        toTarget.y = 0; // Ignorar diferencias en Y para movimiento horizontal

        float distance = toTarget.magnitude;

        // Si estamos muy cerca, no aplicar fuerza
        if (distance <= stopDistance)
            return Vector3.zero;

        // 1. Calculamos la velocidad deseada (un vector que apunta hacia el objetivo a máxima velocidad)
        Vector3 desiredVelocity = toTarget.normalized * agent.maxSpeed;

        // 2. La fuerza de dirección ("steering") es la diferencia entre la velocidad deseada y la actual
        Vector3 steeringForce = desiredVelocity - agent.Velocity;

        return steeringForce;
    }

    public void SetTarget()
    {
        if (GetComponent<PrioritySteering>() != null)
        {
            target = GetComponent<PrioritySteering>().GetTarget();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}