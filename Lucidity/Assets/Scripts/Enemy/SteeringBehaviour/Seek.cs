using UnityEngine;

public class Seek : SteeringBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private PrioritorySteering ps;

    private void Start()
    {
        target = GameManager.PlayerRef.transform;
    }

    public override Vector3 CalculateForce(Agent agent)
    {
        if(target == null) return Vector3.zero;

        Vector3 desiredVelocity = (target.position - agent.Position).normalized * agent.maxSpeed;
        Vector3 steeringForce = desiredVelocity - agent.Velocity;

        return steeringForce;
    }
}
