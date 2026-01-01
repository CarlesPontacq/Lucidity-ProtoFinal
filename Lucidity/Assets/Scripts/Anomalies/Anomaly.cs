using UnityEngine;

public abstract class Anomaly : MonoBehaviour, ICameraModeListener
{
    [SerializeField] private string anomalyId;
    public string Id => anomalyId;

    [Header("Normal Object")]
    [SerializeField] private GameObject normalObjectOverride;

    protected GameObject NormalObject => normalObjectOverride != null ? normalObjectOverride : transform.parent?.gameObject;

    public void Activate() => OnActivate();
    public void Deactivate() => OnDeactivate();

    protected virtual void OnActivate()
    {
        SetNormalRenderersEnabled(false);
    }

    protected virtual void OnDeactivate()
    {
        SetNormalRenderersEnabled(true);
    }

    private void SetNormalRenderersEnabled(bool enabled)
    {
        if (NormalObject == null) return;

        var renderers = NormalObject.GetComponentsInChildren<Renderer>(true);
        if (renderers == null || renderers.Length == 0) return;

        foreach (var r in renderers)
            r.enabled = enabled;
    }


    public virtual void OnCameraModeActivated(CameraMode mode)
    {
        if(mode is DocumentationMode)
        {
            OnDeactivate();
        }
    }

    public virtual void OnCameraModeDeactivated(CameraMode mode)
    {
        if (mode is DocumentationMode)
        {
            OnActivate();
        }
    }
}
