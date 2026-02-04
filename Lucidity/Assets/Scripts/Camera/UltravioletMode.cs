using UnityEngine;

public class UltravioletMode : CameraMode
{
    [Header("Ultraviolet Mode")]
    [Range(0f, 100f)]
    public float uvBattery = 100f;
    private float minUvBattery = 0f;
    [SerializeField] private float maxUvBattery = 100f;

    [SerializeField] private float uvLightExhaustionSpeed = .05f;
    [SerializeField] private float uvLightRecoverySpeed = .005f;

    public bool isUvLightOn = false;

    private Light uvLight;

    protected void Start()
    {
        base.Start();
    }

    protected void Update()
    {
        base.Update();

        if (uvLight == null) return;

        if (isUvLightOn)
        {
            uvBattery -= uvLightExhaustionSpeed;
            uvBattery = Mathf.Clamp(uvBattery, minUvBattery, maxUvBattery);

            if(uvBattery <= minUvBattery)
            {
                uvLight.enabled = false;
                isUvLightOn = uvLight.enabled;
            }
        }
        else
        {
            uvBattery += uvLightRecoverySpeed;
            uvBattery = Mathf.Clamp(uvBattery, minUvBattery, maxUvBattery);
            uvLight.enabled = false;
        }
    }

    //Funcion para activar la camara
    public override void ActivateMode()
    {
        uvLight = GetComponent<Light>();
        base .ActivateMode();
    }

    //Funcion para desactivar la camara
    public override void DeactivateMode()
    {
        base.DeactivateMode();
    }

    public override bool PerformCameraAction() 
    {
        if (uvLight == null) return false;

        uvLight.enabled = !uvLight.isActiveAndEnabled;

        isUvLightOn = uvLight.enabled;

        return true;
    }

    protected override void OnActivated() { }

    protected override void OnDeactivated() { }
}
