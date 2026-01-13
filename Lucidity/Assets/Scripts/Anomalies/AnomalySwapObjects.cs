using UnityEngine;

public class AnomalySwapObjects : Anomaly
{
    [SerializeField] private GameObject normalRoot;   
    [SerializeField] private GameObject anomalyRoot;

    private int defaultLayer = 0;          
    private int normalObjectLayer = 6;     
    protected override void OnActivate()
    {
        base.OnActivate();

        if (!IsSpawnedThisLoop) return;

        if (IsSpawnedThisLoop)
            SetLayerRecursive(normalRoot, normalObjectLayer);
        else
            SetLayerRecursive(normalRoot, defaultLayer);

        if (normalRoot) normalRoot.SetActive(false);
        if (anomalyRoot) anomalyRoot.SetActive(true);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();

        if (!IsSpawnedThisLoop) return;

        // Vuelve al estado normal visible
        if(IsSpawnedThisLoop)
            SetLayerRecursive(normalRoot, normalObjectLayer);
        else
            SetLayerRecursive(normalRoot, defaultLayer);

        if (normalRoot) normalRoot.SetActive(true);
        if (anomalyRoot) anomalyRoot.SetActive(false);
    }

    public override void MarkSpawned()
    {
        base.MarkSpawned();

        // Cuando SE SPAWNEA
        normalRoot.layer = normalObjectLayer;
        foreach (Transform child in normalRoot.transform)
        {
            child.gameObject.layer = normalObjectLayer;
        }

        normalRoot.SetActive(true);
        anomalyRoot.SetActive(false);
    }

    public override void MarkUnspawned()
    {
        base.MarkUnspawned();

        // Cuando SE SPAWNEA
        normalRoot.layer = defaultLayer;
        foreach(Transform child in normalRoot.transform)
        {
            child.gameObject.layer = defaultLayer;
        }

        normalRoot.SetActive(true);
        anomalyRoot.SetActive(false);
    }

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        if (obj == null) return;

        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursive(child.gameObject, layer);

        Debug.Log(obj.layer);
    }
}
