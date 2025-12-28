using UnityEngine;

public abstract class Anomaly : MonoBehaviour, ICameraModeListener
{
    [SerializeField] private string anomalyId;
    public string Id => anomalyId;

    public void Activate() => OnActivate();
    public void Deactivate() => OnDeactivate();

    protected virtual void OnActivate() { }
    protected virtual void OnDeactivate() { }

    public virtual void OnCameraModeActivated(CameraMode mode)
    {
        if(mode is DocumentationMode)
        {
            OnActivate();
        }
    }

    public virtual void OnCameraModeDeactivated(CameraMode mode)
    {
        if (mode is DocumentationMode)
        {
            OnDeactivate();
        }
    }
}
