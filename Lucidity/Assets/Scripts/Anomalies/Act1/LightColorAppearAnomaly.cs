using UnityEngine;

public class LightColorAppearAnomaly : Anomaly
{
    [Header("Scene References")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private CameraManager cameraManager;

    private GameObject redOutside;

    [Header("New Outside Renderer")]
    private Renderer newOutsideRenderer;
    private int newOutsideMaterialIndex = 0;

    [Header("Colors")]
    [SerializeField] private Color anomalyLightColor = Color.red;
    [SerializeField] private Color originalLightColor;

    [SerializeField] private Color anomalyWindowBaseColor = Color.red;
    [SerializeField] private Color anomalyWindowEmissionColor = Color.red;
    [SerializeField] private float emissionIntensity = 2f;
    [SerializeField] private float originalEmissionIntensity = 1f;

    private bool prevLookingThroughtCamera = false;

    private Material newOutsideMatInstance;
    private Color originalBaseColor;
    private Color originalEmissionColor;
    private bool hadEmissionKeyword;
    private LayerMask resetLayer = 0;
    private LayerMask anomalyLayer = 3;
    private LayerMask normalObjectLayer = 6;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");     // URP Lit
    private static readonly int ColorId = Shader.PropertyToID("_Color");         // fallback
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor"); // URP/Lit
    private const string EmissionKeyword = "_EMISSION";

    private void Awake()
    {

    }

    private void Update()
    {
        if (!IsSpawnedThisLoop || cameraManager == null) return;

        if (prevLookingThroughtCamera == cameraManager.lookingThroughCamera) return;

        if (cameraManager.lookingThroughCamera)
        {
            ShowAnomalyLight();
        }
        else
        {
            ShowNormalLight();
        }

        prevLookingThroughtCamera = cameraManager.lookingThroughCamera;
    }

    private void CacheWindowMaterial()
    {
        if (redOutside == null) return;

        newOutsideRenderer = redOutside.GetComponent<Renderer>();

        var mats = newOutsideRenderer.materials; 
        if (mats == null || mats.Length == 0) return;

        if (newOutsideMaterialIndex < 0 || newOutsideMaterialIndex >= mats.Length)
        {
            Debug.LogWarning($"[LightColorAppearAnomaly] windowMaterialIndex fuera de rango. " +
                             $"Renderer={newOutsideRenderer.name} mats={mats.Length}");
            return;
        }

        newOutsideMatInstance = mats[newOutsideMaterialIndex];
        if (newOutsideMatInstance == null) return;

        if (newOutsideMatInstance.HasProperty(BaseColorId))
            originalBaseColor = newOutsideMatInstance.GetColor(BaseColorId);
        else if (newOutsideMatInstance.HasProperty(ColorId))
            originalBaseColor = newOutsideMatInstance.GetColor(ColorId);
        else
            originalBaseColor = Color.white;

        hadEmissionKeyword = newOutsideMatInstance.IsKeywordEnabled(EmissionKeyword);
        if (newOutsideMatInstance.HasProperty(EmissionColorId))
        {
            originalEmissionColor = newOutsideMatInstance.GetColor(EmissionColorId);
        }
        else
            originalEmissionColor = Color.black;
    }

    protected override void OnActivate()
    {
        MarkSpawned();

        if (base.NormalObject != null)
        {
            redOutside = Instantiate(base.NormalObject);
            SetOutsideRenderingMask();
            CacheWindowMaterial();
        }

        ApplyWindowColors(anomalyWindowBaseColor, anomalyWindowEmissionColor, emissionIntensity, true);
    }

    void ShowAnomalyLight()
    {
        if (directionalLight != null)
        {
            directionalLight.color = anomalyLightColor;
        }
    }

    void ShowNormalLight()
    {
        if (directionalLight != null)
        {
            directionalLight.color = originalLightColor;
        }
    }

    private void SetOutsideRenderingMask()
    {
        base.NormalObject.layer = normalObjectLayer;
        redOutside.layer = anomalyLayer;
    }

    protected override void OnDeactivate()
    {
        MarkUnspawned();
        ShowNormalLight();

        if (redOutside != null)
            Destroy(redOutside);

        base.NormalObject.layer = resetLayer;
    }

    private void ApplyWindowColors(Color baseColor, Color emissionColor, float intensity, bool enableEmission)
    {
        if (newOutsideMatInstance == null)
        {
            CacheWindowMaterial();
            if (newOutsideMatInstance == null) return;
        }

        if (newOutsideMatInstance.HasProperty(BaseColorId))
            newOutsideMatInstance.SetColor(BaseColorId, baseColor);
        else if (newOutsideMatInstance.HasProperty(ColorId))
            newOutsideMatInstance.SetColor(ColorId, baseColor);

        if (newOutsideMatInstance.HasProperty(EmissionColorId))
        {
            if (enableEmission) newOutsideMatInstance.EnableKeyword(EmissionKeyword);
            else newOutsideMatInstance.DisableKeyword(EmissionKeyword);

            newOutsideMatInstance.SetColor(EmissionColorId, emissionColor * intensity);
        }
    }
}