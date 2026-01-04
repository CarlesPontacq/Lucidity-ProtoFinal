using System.Collections;
using UnityEngine;

public class SetUpAnomalyLightsLight : MonoBehaviour
{
    [SerializeField] private Light normalLight;


    void Start()
    {
        normalLight.enabled = false;
    }
}
