using UnityEngine;
using UnityEngine.AI;

public class NavMovement : MonoBehaviour
{
    [SerializeField] public NavMeshAgent agent;
    private Vector3 playerPosition;

    void Start()
    {
        //Solucion Opcion 2: agent.updatePosition = false; agent.updateRotation = false;
    }

    void Update()
    {
        if (GameManager.PlayerRef == null) return;

        playerPosition = GameManager.PlayerRef.transform.position;
        agent.SetDestination(playerPosition);
        Debug.Log("NavMeshAgent velocity: " + agent.velocity.magnitude);

        // Solucion Opcion 1: agent.velocity = agent.desiredVelocity.normalized * agent.speed;
    }
}