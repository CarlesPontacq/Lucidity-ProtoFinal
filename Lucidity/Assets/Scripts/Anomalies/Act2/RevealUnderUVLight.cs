using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RevealUnderUVLight : MonoBehaviour
{
    [SerializeField] private DecalProjector decalProjector;
    [SerializeField] private Light uvLight;

    static readonly int LightPosID = Shader.PropertyToID("_LightPos");
    static readonly int LightDirID = Shader.PropertyToID("_LightDir");
    static readonly int LightAngleID = Shader.PropertyToID("_LightAngle");
    static readonly int LightEnabledID = Shader.PropertyToID("_LightEnabled");

    Material runtimeMaterial;

    void Awake()
    {
        // Creamos instancia para no modificar el asset
        runtimeMaterial = Instantiate(decalProjector.material);
        decalProjector.material = runtimeMaterial;
    }

    void Update()
    {
        if (!runtimeMaterial || !uvLight) return;

        bool lightOn = uvLight.enabled;

        runtimeMaterial.SetFloat(LightEnabledID, lightOn ? 1f : 0f);

        if (!lightOn) return;

        runtimeMaterial.SetVector(LightPosID, uvLight.transform.position);
        runtimeMaterial.SetVector(LightDirID, uvLight.transform.forward);
        runtimeMaterial.SetFloat(LightAngleID, uvLight.spotAngle);
    }
}