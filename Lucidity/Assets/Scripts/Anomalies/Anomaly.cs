using UnityEngine;

public abstract class Anomaly : MonoBehaviour
{
    [SerializeField] private string anomalyId;
    public string Id => anomalyId;

    public void Activate() => OnActivate();
    public void Deactivate() => OnDeactivate();

    protected virtual void OnActivate() { }
    protected virtual void OnDeactivate() { }
}
