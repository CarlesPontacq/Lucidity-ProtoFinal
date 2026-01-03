using UnityEngine;

public class SecurityCameraVisibility : MonoBehaviour
{
    public bool setCameraAnomalyVisible;
    [SerializeField] private MeshRenderer anomalyMeshRenderer;

    void Update()
    {
        anomalyMeshRenderer.enabled = setCameraAnomalyVisible;
    }
}
