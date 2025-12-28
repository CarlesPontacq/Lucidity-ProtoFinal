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
        if (NormalObject != null)
        {
            NormalObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    protected virtual void OnDeactivate() 
    {
        if (NormalObject != null)
        {
            NormalObject.GetComponent<MeshRenderer>().enabled = true;
        }
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
