using UnityEngine;

public class RevealUnderUVLight : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] private Light uvLight;

    static readonly int LightPosID = Shader.PropertyToID("_LightPos");
    static readonly int LightDirID = Shader.PropertyToID("_LightDir");
    static readonly int LightAngleID = Shader.PropertyToID("_LightAngle");
    static readonly int LightEnabledID = Shader.PropertyToID("_LightEnabled");

    void Update()
    {
        if (!mat || !uvLight) return;

        bool enabled = uvLight.enabled;

        mat.SetFloat(LightEnabledID, enabled ? 1f : 0f);

        if (!enabled) return;

        mat.SetVector(LightPosID, uvLight.transform.position.normalized);
        mat.SetVector(LightDirID, uvLight.transform.forward.normalized);
        mat.SetFloat(LightAngleID, uvLight.spotAngle);
    }

}
