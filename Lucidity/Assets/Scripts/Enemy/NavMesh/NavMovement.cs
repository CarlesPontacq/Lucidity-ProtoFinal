using UnityEngine;
using UnityEngine.AI;

public class NavMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public NavMeshAgent agent;
    private Vector3 playerPosition;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float speedCorrectionFactor = 2f;

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

        if(animator != null)
        {
           float currentSpeed = agent.velocity.magnitude;

            animator.speed = currentSpeed / speedCorrectionFactor;
        }

        // Solucion Opcion 1: agent.velocity = agent.desiredVelocity.normalized * agent.speed;
    }
}