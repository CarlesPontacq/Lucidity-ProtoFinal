using UnityEngine;

public class AnomalySwapSetLayer : MonoBehaviour
{

    [SerializeField] private GameObject normalRoot;
    [SerializeField] private GameObject anomalyRoot;

    private int defaultLayer = 0;
    private int normalObjectLayer = 6;

    void Start()
    {
        
    }

    void Update()
    {
        if (anomalyRoot.activeSelf)
        {
            foreach(Transform child in normalRoot.transform)
            {
                child.gameObject.layer = normalObjectLayer;
            }
        }
        else
        {
            foreach (Transform child in normalRoot.transform)
            {
                child.gameObject.layer = defaultLayer;
            }
        }
    }
}
