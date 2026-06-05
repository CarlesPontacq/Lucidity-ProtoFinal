using UnityEngine;

public class SecurityCameraLookAtPlayer : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private Vector3 rotationOffset;

    void Start()
    {
        if(player == null)
        {
            player = GameManager.PlayerRef.transform;
        }    
    }


    void Update()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            targetRotation * Quaternion.Euler(rotationOffset), 
            rotationSpeed * Time.deltaTime);
    }
}
