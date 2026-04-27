using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Agent3D : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 5f;
    public float maxForce = 10f;
    public float turnRateDeg = 180f;

    [Header("Behaviour Component")]
    public SteeringBehaviour behaviour;

    public Vector3 Position => rb.position;
    public Vector3 Velocity => rb.linearVelocity;

    [SerializeField] private Rigidbody rb;


    void FixedUpdate()
    {
        if (behaviour != null)
        {
            Vector3 steeringForce = behaviour.CalculateForce(this);

            steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);

            rb.AddForce(steeringForce);

            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);

            if (rb.linearVelocity.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(rb.linearVelocity.normalized, Vector3.up);
                Quaternion newRotation = Quaternion.RotateTowards(rb.rotation, targetRotation, turnRateDeg * Time.fixedDeltaTime);
                rb.MoveRotation(newRotation);
            }
        }
    }

    public void SetBehaviour(SteeringBehaviour newBehaviour)
    {
        behaviour = newBehaviour;
    }
}