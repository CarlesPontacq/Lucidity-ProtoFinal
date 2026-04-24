using UnityEngine;
using UnityEngine.AI;

public class NavMovement : MonoBehaviour
{
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] private float updateRate = 0.2f;
    [SerializeField] private float pathRecalculationDistance = 2.0f;

    private Vector3 lastPlayerPosition;
    private float nextUpdateTime;


    void Update()
    {
        if (GameManager.PlayerRef == null) return;

        // Actualizar destino constantemente (persecución continua)
        Vector3 playerPosition = GameManager.PlayerRef.transform.position;
        agent.SetDestination(playerPosition);

        // Opcional: Si quieres que mire directamente al jugador (más allá de la dirección de movimiento)
        // Esto es útil si el agente tiene una "cabeza" o "cara" que debe mirar al jugador
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            //LookAtPlayer();
        }
    }

    void LookAtPlayer()
    {
        Vector3 directionToPlayer = GameManager.PlayerRef.transform.position - transform.position;
        directionToPlayer.y = 0; // Mantener rotación horizontal

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            // Rotación suave pero rápida
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                agent.angularSpeed * Time.deltaTime
            );
        }
    }

    // Opcional: Debug para visualizar la ruta
    void OnDrawGizmosSelected()
    {
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.yellow;
            Vector3[] corners = agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Gizmos.DrawLine(corners[i], corners[i + 1]);
            }
        }
    }
}