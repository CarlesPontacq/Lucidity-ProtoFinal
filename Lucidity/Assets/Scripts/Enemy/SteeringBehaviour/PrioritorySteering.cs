using UnityEngine;

public class PrioritorySteering : MonoBehaviour
{
    public Transform playerTarget;


    void Start()
    {
        playerTarget = GameManager.PlayerRef.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
