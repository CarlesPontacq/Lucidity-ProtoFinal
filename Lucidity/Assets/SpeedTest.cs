using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class SpeedTest : MonoBehaviour
{
    private NavMeshAgent agent;
    public TextMeshProUGUI text;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(agent != null)
            text.text = agent.velocity.magnitude.ToString();
        else
            agent = FindAnyObjectByType<NavMeshAgent>();
    }
}
