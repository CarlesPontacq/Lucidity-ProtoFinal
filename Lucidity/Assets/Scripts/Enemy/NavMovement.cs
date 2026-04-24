using UnityEngine;
using UnityEngine.AI;

public class NavMovement : MonoBehaviour
{
    [SerializeField] public NavMeshAgent agent;

    void Update()
    {
        if (GameManager.PlayerRef == null) return;

        Vector3 playerPosition = GameManager.PlayerRef.transform.position;
        agent.SetDestination(playerPosition);
    }
}