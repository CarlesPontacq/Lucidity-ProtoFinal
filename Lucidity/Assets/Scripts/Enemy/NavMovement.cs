using UnityEngine;
using UnityEngine.AI;

public class NavMovement : MonoBehaviour
{
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public Vector3 playerPosition;

    void Update()
    {
        if (GameManager.PlayerRef == null) return;

        playerPosition = GameManager.PlayerRef.transform.position;
        agent.SetDestination(playerPosition);
    }
}