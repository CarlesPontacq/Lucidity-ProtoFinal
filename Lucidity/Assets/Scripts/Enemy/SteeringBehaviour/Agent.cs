using UnityEngine;

public class Agent : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float maxForce = 10f;

    public SteeringBehaviour behaviour;

    public Vector3 Position => rb.position;
    public Vector3 Velocity => rb.linearVelocity;

    [SerializeField] private Rigidbody rb;

    void FixedUpdate()
    {
        if(behaviour != null)
        {
            Vector3 steeringForce = behaviour.CalculateForce(this);

            steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);

            rb.AddForce(steeringForce);

            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);

            if(rb.linearVelocity.magnitude > 0.1f)
            {
                transform.up = rb.linearVelocity.normalized;
            }
        }
    }
}
