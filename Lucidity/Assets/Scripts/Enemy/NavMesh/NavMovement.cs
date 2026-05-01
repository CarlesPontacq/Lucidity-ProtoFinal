using UnityEngine;
using UnityEngine.AI;

public class NavMovement : MonoBehaviour
{
    [SerializeField] public NavMeshAgent agent;
    private Vector3 playerPosition;

    void Update()
    {
        if (GameManager.PlayerRef == null) return;

        playerPosition = GameManager.PlayerRef.transform.position;
        agent.SetDestination(playerPosition);
    }
}