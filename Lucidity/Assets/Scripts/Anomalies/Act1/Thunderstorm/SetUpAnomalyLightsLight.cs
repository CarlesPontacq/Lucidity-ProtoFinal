using System.Collections;
using UnityEngine;

public class SetUpAnomalyLightsLight : MonoBehaviour
{
    [SerializeField] private AnomalySwapObjects swap;


    void Awake()
    {
        swap.GetNormalObjectOverride().GetComponent<Light>().enabled = false;
    }
}
