using UnityEngine;

public class LightColorAppearAnomaly : Anomaly
{
    [Header("Scene References")]
    [SerializeField] private Light directionalLight;

    [Header("Window Renderer")]
    [SerializeField] private Renderer windowRenderer;
    [SerializeField] private int windowMaterialIndex = 0;

    [Header("Colors")]
    [SerializeField] private Color anomalyLightColor = Color.red;

    [SerializeField] private Color anomalyWindowBaseColor = Color.red;
    [SerializeField] private Color anomalyWindowEmissionColor = Color.red;
    [SerializeField] private float emissionIntensity = 2f;
    [SerializeField] private float originalEmissionIntensity = 1f;

    private Color originalLightColor;

    private Material windowMatInstance;
    private Color originalBaseColor;
    private Color originalEmissionColor;
    private bool hadEmissionKeyword;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");     // URP Lit
    private static readonly int ColorId = Shader.PropertyToID("_Color");         // fallback
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor"); // URP/Lit
    private const string EmissionKeyword = "_EMISSION";

    private void Awake()
    {
        if (directionalLight != null)
            originalLightColor = directionalLight.color;

        CacheWindowMaterial();
    }

    private void CacheWindowMaterial()
    {
        if (windowRenderer == null) return;

        var mats = windowRenderer.materials; 
        if (mats == null || mats.Length == 0) return;

        if (windowMaterialIndex < 0 || windowMaterialIndex >= mats.Length)
        {
            Debug.LogWarning($"[LightColorAppearAnomaly] windowMaterialIndex fuera de rango. " +
                             $"Renderer={windowRenderer.name} mats={mats.Length}");
            return;
        }

        windowMatInstance = mats[windowMaterialIndex];
        if (windowMatInstance == null) return;

        if (windowMatInstance.HasProperty(BaseColorId))
            originalBaseColor = windowMatInstance.GetColor(BaseColorId);
        else if (windowMatInstance.HasProperty(ColorId))
            originalBaseColor = windowMatInstance.GetColor(ColorId);
        else
            originalBaseColor = Color.white;

        hadEmissionKeyword = windowMatInstance.IsKeywordEnabled(EmissionKeyword);
        if (windowMatInstance.HasProperty(EmissionColorId))
        {
            originalEmissionColor = windowMatInstance.GetColor(EmissionColorId);
        }
        else
            originalEmissionColor = Color.black;
    }

    protected override void OnActivate()
    {
        if (directionalLight != null)
            directionalLight.color = anomalyLightColor;

        ApplyWindowColors(anomalyWindowBaseColor, anomalyWindowEmissionColor, emissionIntensity, true);
    }

    protected override void OnDeactivate()
    {
        if (directionalLight != null)
            directionalLight.color = originalLightColor;

        ApplyWindowColors(originalBaseColor, originalEmissionColor, originalEmissionIntensity, hadEmissionKeyword);
    }

    private void ApplyWindowColors(Color baseColor, Color emissionColor, float intensity, bool enableEmission)
    {
        if (windowMatInstance == null)
        {
            CacheWindowMaterial();
            if (windowMatInstance == null) return;
        }

        if (windowMatInstance.HasProperty(BaseColorId))
            windowMatInstance.SetColor(BaseColorId, baseColor);
        else if (windowMatInstance.HasProperty(ColorId))
            windowMatInstance.SetColor(ColorId, baseColor);

        if (windowMatInstance.HasProperty(EmissionColorId))
        {
            if (enableEmission) windowMatInstance.EnableKeyword(EmissionKeyword);
            else windowMatInstance.DisableKeyword(EmissionKeyword);

            Color emissionIntensity = emissionColor * intensity;

            windowMatInstance.SetColor(EmissionColorId, emissionColor * intensity);
        }
    }
}