using UnityEngine;

public class ObstacleAvoidance3D : SteeringBehaviour
{
    [Header("Avoidance Settings")]
    [SerializeField] private float avoidDistance = 4.0f;
    [SerializeField] private float avoidRadius = 0.35f;
    [SerializeField] private float avoidStrength = 5.0f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Ray Configuration")]
    [SerializeField] private float rayHeight = 0.5f;
    [SerializeField] private int rayCount = 5;
    [SerializeField] private float rayAngleSpread = 60f;

    public override Vector3 CalculateForce(Agent3D agent)
    {
        Vector3 origin = agent.Position + Vector3.up * rayHeight;
        Vector3 velocityDir = agent.Velocity.magnitude > 0.1f ? agent.Velocity.normalized : agent.transform.forward;

        Vector3 avoidanceForce = Vector3.zero;
        int hitCount = 0;

        // Generar rayos en abanico
        Vector3[] rayDirections = GetRayDirections(velocityDir);

        foreach (Vector3 dir in rayDirections)
        {
            if (Physics.SphereCast(origin, avoidRadius, dir, out RaycastHit hit, avoidDistance, obstacleMask, QueryTriggerInteraction.Ignore))
            {
                // Calcular dirección de escape perpendicular al obstáculo
                Vector3 away = Vector3.Cross(Vector3.up, hit.normal).normalized;

                // Si el rayo central golpeó, también considerar dirección opuesta
                if (dir == velocityDir)
                {
                    away = -hit.normal;
                    away.y = 0;
                    away.Normalize();
                }

                // Ponderar por distancia (más fuerza si está más cerca)
                float weight = (avoidDistance - hit.distance) / avoidDistance;
                avoidanceForce += away * avoidStrength * weight;
                hitCount++;
            }
        }

        if (hitCount > 0)
        {
            avoidanceForce /= hitCount;
            avoidanceForce.y = 0f;
            return Vector3.ClampMagnitude(avoidanceForce, agent.maxForce);
        }

        return Vector3.zero;
    }

    private Vector3[] GetRayDirections(Vector3 forwardDir)
    {
        Vector3[] directions = new Vector3[rayCount];

        if (rayCount == 1)
        {
            directions[0] = forwardDir;
            return directions;
        }

        float angleStep = rayAngleSpread / (rayCount - 1);
        float startAngle = -rayAngleSpread / 2;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = startAngle + (angleStep * i);
            directions[i] = Quaternion.Euler(0, angle, 0) * forwardDir;
        }

        return directions;
    }

    // Método público para debug visual
    public void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.yellow;
        Agent3D agent = GetComponent<Agent3D>();
        if (agent != null && agent.Velocity.magnitude > 0.1f)
        {
            Vector3 origin = agent.Position + Vector3.up * rayHeight;
            Vector3[] rayDirections = GetRayDirections(agent.Velocity.normalized);

            foreach (Vector3 dir in rayDirections)
            {
                Gizmos.DrawRay(origin, dir * avoidDistance);
            }
        }
    }
}