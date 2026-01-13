using UnityEngine;

public class ExitLightEmissionMapSwitcher : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private int materialIndex = 0;

    [Header("Emission Map Colors")]
    [SerializeField] private Color blockedColor = Color.red;
    [SerializeField] private Color unlockedColor = Color.green;

    [Header("Emission Color (kept constant so map shows)")]
    [SerializeField] private float emissionIntensity = 2f;

    private Material matInstance;
    private Texture2D texRed;
    private Texture2D texGreen;

    private static readonly int EmissionMapId = Shader.PropertyToID("_EmissionMap");
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer == null)
        {
            Debug.LogWarning("ExitLightEmissionMapSwitcher: no Renderer.");
            enabled = false;
            return;
        }

        // Crea material instanciado (no modificar sharedMaterial)
        var mats = targetRenderer.materials;
        if (materialIndex < 0 || materialIndex >= mats.Length)
        {
            Debug.LogWarning("ExitLightEmissionMapSwitcher: materialIndex fuera de rango.");
            enabled = false;
            return;
        }

        matInstance = mats[materialIndex];
        matInstance.EnableKeyword("_EMISSION");
        matInstance.SetColor(EmissionColorId, Color.white * emissionIntensity);

        // Crear 2 texturas 1x1 en runtime
        texRed = Make1x1(blockedColor);
        texGreen = Make1x1(unlockedColor);

        SetCanPass(false); // rojo por defecto
    }

    public void SetCanPass(bool canPass)
    {
        if (matInstance == null) return;

        matInstance.SetTexture(EmissionMapId, canPass ? texGreen : texRed);
        matInstance.EnableKeyword("_EMISSION");
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
