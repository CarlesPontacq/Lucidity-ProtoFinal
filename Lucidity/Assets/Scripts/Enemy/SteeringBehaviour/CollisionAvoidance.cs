using UnityEngine;
using System.Collections.Generic;

public class CollisionAvoidance : SteeringBehaviour
{
    [Header("Collision Avoidance Settings")]
    [SerializeField] private float avoidanceLookahead = 2f;   
    [SerializeField] private float avoidanceDistance = 1f;    
    [SerializeField] private float detectionRadius = 3f;      
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private Quaternion[] directions = { 
        Quaternion.Euler(0, 30, 0),
        Quaternion.Euler(0, -30, 0),
        Quaternion.Euler(0, 60, 0),
        Quaternion.Euler(0, -60, 0),
    };

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;

    private RaycastHit[] hitsBuffer = new RaycastHit[10];

    public override Vector3 CalculateForce(Agent agent)
    {
        Vector3 avoidanceForce = Vector3.zero;
        float closestHitDistance = float.MaxValue;
        RaycastHit closestHit = default(RaycastHit);

        // 1. Raycast hacia adelante (dirección del movimiento)
        Vector3 forwardDir = agent.Velocity.normalized;

        // Si el agente está quieto, usar su dirección forward
        if (forwardDir == Vector3.zero)
            forwardDir = agent.transform.forward;

        // Raycast principal hacia adelante
        if (Physics.Raycast(agent.transform.position, forwardDir, out RaycastHit forwardHit,
            avoidanceLookahead, obstacleMask))
        {
            if (forwardHit.distance < closestHitDistance)
            {
                closestHitDistance = forwardHit.distance;
                closestHit = forwardHit;
            }
        }

        // 2. Raycasts en direcciones alternativas (para detección más amplia)
        foreach (Quaternion rotation in directions)
        {
            Vector3 direction = rotation * forwardDir;

            // Usar NonAlloc para mejor performance
            int hitCount = Physics.RaycastNonAlloc(agent.transform.position, direction,
                hitsBuffer, detectionRadius, obstacleMask);

            for (int i = 0; i < hitCount; i++)
            {
                float distance = hitsBuffer[i].distance;
                if (distance < closestHitDistance)
                {
                    closestHitDistance = distance;
                    closestHit = hitsBuffer[i];
                }
            }
        }

        // 3. Esfera de detección para obstáculos cercanos
        int sphereHitCount = Physics.SphereCastNonAlloc(agent.transform.position,
            avoidanceDistance, forwardDir, hitsBuffer, detectionRadius, obstacleMask);

        for (int i = 0; i < sphereHitCount; i++)
        {
            float distance = hitsBuffer[i].distance;
            if (distance < closestHitDistance && distance > 0.1f)
            {
                closestHitDistance = distance;
                closestHit = hitsBuffer[i];
            }
        }

        // 4. Calcular la fuerza de evasión si encontramos un obstáculo
        if (closestHit.collider != null)
        {
            // Factor basado en la distancia (más cerca = más fuerza)
            float distanceFactor = 1.0f - Mathf.Clamp01(closestHit.distance / detectionRadius);

            // Dirección perpendicular al obstáculo
            Vector3 avoidanceDirection = Vector3.Cross(Vector3.up, closestHit.normal).normalized;

            // Alternativa: usar la normal del obstáculo
            // Vector3 avoidanceDirection = (closestHit.point - agent.transform.position).normalized;
            // avoidanceDirection.y = 0; // Mantener en plano XZ para movimiento terrestre

            // Calcular fuerza con lookahead dinámico
            float dynamicStrength = Mathf.Lerp(agent.maxSpeed, agent.maxSpeed * 3f, distanceFactor);
            avoidanceForce = avoidanceDirection * dynamicStrength;

            // Añadir componente de frenado si es muy cercano
            if (closestHit.distance < avoidanceDistance)
            {
                Vector3 brakingForce = -agent.Velocity * (1 - distanceFactor) * 2f;
                avoidanceForce += brakingForce;
            }

            // Limitar la fuerza máxima
            avoidanceForce = Vector3.ClampMagnitude(avoidanceForce, agent.maxForce);
        }

        return avoidanceForce;
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        // Visualizar el lookahead
        Gizmos.color = Color.yellow;
        if (Application.isPlaying && GetComponent<Agent>() != null)
        {
            Agent agent = GetComponent<Agent>();
            Vector3 forwardDir = agent.Velocity.normalized;
            if (forwardDir == Vector3.zero)
                forwardDir = transform.forward;

            Gizmos.DrawRay(transform.position, forwardDir * avoidanceLookahead);

            // Visualizar direcciones alternativas
            Gizmos.color = Color.green;
            foreach (Quaternion rotation in directions)
            {
                Vector3 direction = rotation * forwardDir;
                Gizmos.DrawRay(transform.position, direction * detectionRadius);
            }
        }
        else
        {
            Gizmos.DrawRay(transform.position, transform.forward * avoidanceLookahead);
        }

        // Visualizar radio de detección
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Visualizar distancia de evasión
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, avoidanceDistance);
    }
}