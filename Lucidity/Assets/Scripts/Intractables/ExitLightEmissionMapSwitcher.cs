using UnityEngine;

public class ExitLightEmissionMapSwitcher : MonoBehaviour
{
    [Header("Renderer / Material")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private int materialIndex = 1; // bombilla

    [Header("Point Light (optional)")]
    [SerializeField] private Light pointLight;
    [SerializeField] private bool changeLightIntensity = false;
    [SerializeField] private float blockedIntensity = 2f;
    [SerializeField] private float unlockedIntensity = 2f;

    [Header("Emission Map Colors")]
    [SerializeField] private Color blockedColor = Color.red;
    [SerializeField] private Color unlockedColor = Color.green;

    [Header("Emission Strength (HDR)")]
    [SerializeField] private float emissionIntensity = 2f;

    private Texture2D texRed;
    private Texture2D texGreen;

    private MaterialPropertyBlock mpb;

    private static readonly int EmissionMapId = Shader.PropertyToID("_EmissionMap");
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (pointLight == null)
            pointLight = GetComponentInChildren<Light>();

        if (targetRenderer == null)
        {
            Debug.LogWarning("[ExitLamp] No Renderer encontrado.");
            enabled = false;
            return;
        }

        var mats = targetRenderer.sharedMaterials;
        if (materialIndex < 0 || materialIndex >= mats.Length || mats[materialIndex] == null)
        {
            Debug.LogWarning($"[ExitLamp] materialIndex fuera de rango o material null. index={materialIndex}, mats={mats.Length}");
            enabled = false;
            return;
        }

        mats[materialIndex].EnableKeyword("_EMISSION");

        mpb = new MaterialPropertyBlock();

        texRed = Make1x1(blockedColor);
        texGreen = Make1x1(unlockedColor);

        SetCanPass(false); 
    }

    public void SetCanPass(bool canPass)
    {
        if (targetRenderer == null) return;

        Color emissionBase = Color.white * emissionIntensity;

        mpb.Clear();
        mpb.SetTexture(EmissionMapId, canPass ? texGreen : texRed);
        mpb.SetColor(EmissionColorId, emissionBase);

        targetRenderer.SetPropertyBlock(mpb, materialIndex);

        // --- Point Light
        if (pointLight != null)
        {
            pointLight.color = canPass ? unlockedColor : blockedColor;

            if (changeLightIntensity)
                pointLight.intensity = canPass ? unlockedIntensity : blockedIntensity;
        }
    }

    private Texture2D Make1x1(Color c)
    {
        var t = new Texture2D(1, 1, TextureFormat.RGBA32, false, false);
        t.SetPixel(0, 0, c);
        t.Apply();
        t.wrapMode = TextureWrapMode.Clamp;
        t.filterMode = FilterMode.Point;
        return t;
    }
}
