using System.Collections;
using UnityEngine;

public class SetUpAnomalyLightsLight : MonoBehaviour
{
    [SerializeField] private AnomalySwapObjects swap;


    void Start()
    {
        swap.GetNormalObjectOverride().GetComponent<Light>().enabled = false;
    }
}
