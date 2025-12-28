using System;
using System.Collections;
using UnityEngine;

public class DocumentationMode : CameraMode
{
    public int maxReels = 5;
    public int currentReels;

    [SerializeField] private float flasDuration = 0.3f;

    private Coroutine flashCoroutine;

    private Camera docCamera;

    void Start()
    {
        base.Start();

        currentReels = maxReels;
        isUnlocked = true;
    }


    void Update()
    {
        base.Update();
    }

    protected override void OnActivated()
    {
        docCamera = GetComponent<Camera>();
        base.OnActivated();

        if(flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();

        if(flashCoroutine != null)
            StopCoroutine(flashCoroutine);
    }

    private IEnumerator FlashCoroutine()
    {
        yield return new WaitForSeconds(flasDuration);

        CaptureAnomalies();

        FindObjectOfType<CameraManager>().DeactivateMode();
    }

    private void CaptureAnomalies()
    {
        var anomalyManager = FindObjectOfType<AnomalyManager>();
        if (anomalyManager == null || docCamera == null) return;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(docCamera);

        foreach(var anomaly in FindObjectsOfType<Anomaly>())
        {
            Collider col = anomaly.GetComponentInChildren<Collider>();
            if (col == null) continue;

            if(GeometryUtility.TestPlanesAABB(planes, col.bounds))
            {
                anomalyManager.RegisterDocumentation(anomaly);
            }
        }
    }
}
