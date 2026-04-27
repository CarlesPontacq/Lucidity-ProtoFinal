using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour
{
    public abstract Vector3 CalculateForce(Agent3D agent);
}