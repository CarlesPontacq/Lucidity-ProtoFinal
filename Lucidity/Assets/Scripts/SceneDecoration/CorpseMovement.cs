using UnityEngine;

public class CorpseMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;

    [Header("Distance")]
    public float activeDistance = 12f;
    public float sleepDistance = 18f;

    [Header("Force")]
    public float impulseForce = 4f;
    public float cooldown = 0.15f;

    [Header("Movement")]
    public float maxVelocity = 4f;
    public float damping = 0.985f;

    private Transform player;
    private float timer;
    private bool isActive = false;

    void Awake()
    {
        rb.useGravity = true;
        rb.isKinematic = true;
        rb.angularDamping = 3f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Start()
    {
        if (GameManager.Instance != null)
            player = GameManager.PlayerRef != null
                ? GameManager.PlayerRef.transform
                : null;
    }


    void Update()
    {
        if (player == null) return;

        float dist = (player.position - transform.position).sqrMagnitude;

        if (!isActive && dist < activeDistance * activeDistance)
        {
            rb.isKinematic = false;
            isActive = true;
        }
        else if (isActive && dist > sleepDistance * sleepDistance)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            isActive = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive || !other.CompareTag("Player")) return;

        ApplyForce(other);
    }

    void OnTriggerStay(Collider other)
    {
        if (!isActive || !other.CompareTag("Player")) return;

        timer += Time.deltaTime;
        if (timer >= cooldown)
        {
            timer = 0f;
            ApplyForce(other);
        }
    }

    void FixedUpdate()
    {
        if (!isActive) return;

        rb.linearVelocity *= damping;
        rb.angularVelocity *= damping;

        if (rb.linearVelocity.magnitude > maxVelocity)
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
    }

    void ApplyForce(Collider other)
    {
        Rigidbody otherRb = other.attachedRigidbody;

        Vector3 playerVel = otherRb != null ? otherRb.linearVelocity : Vector3.zero;

        Vector3 corpseVel = rb.linearVelocity;

        Vector3 relativeVel = playerVel - corpseVel;

        float relSpeed = relativeVel.magnitude;

        Vector3 dir = (transform.position - other.transform.position).normalized;

        dir.y += 0.15f;

        float force = impulseForce * Mathf.Clamp(relSpeed * 1.2f, 0.5f, 6f);

        float dampingFactor = Mathf.Clamp01(1f - corpseVel.magnitude / maxVelocity);

        force *= dampingFactor;

        rb.AddForce(dir.normalized * force, ForceMode.Impulse);
    }
}