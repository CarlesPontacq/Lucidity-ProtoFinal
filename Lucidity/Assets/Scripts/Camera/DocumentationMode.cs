using System;
using System.Collections;
using UnityEngine;

public class DocumentationMode : CameraMode
{
    [Header("Documentation Mode")]
    public int maxReels = 5;
    public int currentReels;

    [SerializeField] private float flasDuration = 0.3f;

    private Coroutine flashCoroutine;

    private Camera docCamera;
    [SerializeField] private ScreenshotManager screenshotManager;
    [SerializeField] private CameraAudioHandler audioHandler;
    [SerializeField] private CameraEventBroadcaster eventBroadcaster;

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
    }

    public override void PerformCameraAction()
    {

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        if (currentReels > 0)
        {
            currentReels--;

            ui.ShowCameraFlash(true);
            flashCoroutine = StartCoroutine(FlashCoroutine());
            eventBroadcaster.NotifyModeActivated();

            if (ui != null)
                ui.ActualizeRemainingReelsIndicator(currentReels);

            audioHandler.PlayPhotoSfx();
        }

        return;
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
    }

    private IEnumerator FlashCoroutine()
    {
        if (screenshotManager != null)
            screenshotManager.CaptureScreenshot();

        yield return new WaitForSeconds(flasDuration);

        eventBroadcaster.NotifyModeDeactivated();

        ui.ShowCameraFlash(false);

        CaptureAnomalies();
    }

    private void CaptureAnomalies()
    {
        var anomalyManager = FindAnyObjectByType<AnomalyManager>();
        if (anomalyManager == null || docCamera == null) return;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(docCamera);

        foreach (var anomaly in FindObjectsByType<Anomaly>(FindObjectsSortMode.None))
        {
            Collider col = anomaly.GetComponentInChildren<Collider>();
            if (col == null) continue;

            if (GeometryUtility.TestPlanesAABB(planes, col.bounds) && CheckIfAnomalyIsSpawned(anomaly))
            {
                anomalyManager.RegisterDocumentation(anomaly);
            }
        }
    }

    private bool CheckIfAnomalyIsSpawned(Anomaly anomaly)
    {
        var anomalyManager = FindAnyObjectByType<AnomalyManager>();
        if (anomalyManager == null) return false;

        foreach(Anomaly a in anomalyManager.GetSpawnedEnemiesThisLoop())
        {
            if(a == anomaly) return true;
        }

        return false;
    }

    public void ResetReels()
    {
        currentReels = maxReels;
        CameraUIHandler ui = FindAnyObjectByType<CameraUIHandler>();
        if (ui != null)
            ui.ActualizeRemainingReelsIndicator(currentReels);
    }

    public override void LookThroughCamera(bool look)
    {
        ui.ShowDocumentationCameraAspect(look);
    }
}
