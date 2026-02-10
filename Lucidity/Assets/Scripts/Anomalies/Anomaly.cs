using UnityEngine;

public abstract class Anomaly : MonoBehaviour, ICameraModeListener
{
    [SerializeField] private string anomalyId;
    public string Id => anomalyId;

    public bool IsSpawnedThisLoop {  get; private set; }

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

    protected virtual void OnDestroy()
    {
        Deactivate();
    }

    private void SetNormalRenderersEnabled(bool enabled)
    {
        if (NormalObject == null) return;

        var renderers = NormalObject.GetComponentsInChildren<Renderer>(true);
        if (renderers == null || renderers.Length == 0) return;

        foreach (var r in renderers)
            r.enabled = enabled;
    }


    public virtual void OnCameraModeActivated()
    {
        if (!IsSpawnedThisLoop) return;

        OnDeactivate();
    }

    public virtual void OnCameraModeDeactivated()
    {
        if (!IsSpawnedThisLoop) return;

        OnActivate();
    }

    public GameObject GetNormalObjectOverride()
    {
        return normalObjectOverride;
    }

    public virtual void MarkSpawned()
    {
        IsSpawnedThisLoop = true;
    }

    public virtual void MarkUnspawned()
    {
        IsSpawnedThisLoop = false;
    }
}
