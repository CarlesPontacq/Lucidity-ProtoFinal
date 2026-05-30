using UnityEngine;

public class LightColorAppearAnomaly : Anomaly
{
    [Header("Scene References")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private CameraManager cameraManager;

    private GameObject redOutside;

    [Header("Colors")]
    [SerializeField] private Material anomalyMaterial;
    [SerializeField] private Color anomalyLightColor = Color.red;
    [SerializeField] private Color originalLightColor;

    private bool prevLookingThroughtCamera = false;

    private LayerMask resetLayer = 0;
    private LayerMask anomalyLayer = 3;
    private LayerMask normalObjectLayer = 6;

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

    protected override void OnActivate()
    {
        MarkSpawned();

        if (base.NormalObject != null)
        {
            redOutside = Instantiate(base.NormalObject);
            SetOutsideRenderingMask();
            redOutside.GetComponent<Renderer>().material = anomalyMaterial;
        }
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
}