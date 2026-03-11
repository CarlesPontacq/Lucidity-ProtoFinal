using System;
using System.Collections;
using UnityEngine;

public class StunMode : CameraMode
{
    [SerializeField] private float flashDuration = 0.3f;

    private Coroutine flashCoroutine;
    private Camera stunCamera;
    [SerializeField] private Camera cameraWithEnemy;
    [SerializeField] private CameraRotation cameraRotation;
    [SerializeField] private ScreenshotManager screenshotManager;


    void Start()
    {
        base.Start();
        screenshotManager = GetComponent<ScreenshotManager>();
    }

    void Update()
    {
        
    }

    protected override void OnActivated()
    {
        stunCamera = GetComponent<Camera>();
        ui.ShowStunCameraEffect(true);
        base.OnActivated();
    }

    public override void PerformCameraAction()
    {
        if(flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        isPerformingAction = true;

        ui.ShowCameraFlash(true);
        flashCoroutine = StartCoroutine(StunFlashCoroutine());
        audioHandler.PlayPhotoSfx();
    }

    private IEnumerator StunFlashCoroutine()
    {
        cameraRotation.SetControlEnabled(false);
        GameManager.Instance.SetPlayerControlEnabled(false);

        if (screenshotManager != null)
            screenshotManager.CaptureStunScreenshots(cameraWithEnemy, stunCamera);


        yield return new WaitForSeconds(flashDuration);

        ui.ShowCameraFlash(false);
        cameraRotation.SetControlEnabled(true);
        GameManager.Instance.SetPlayerControlEnabled(true);

        TryStunEnemy();
        isPerformingAction = false;
    }

    private void TryStunEnemy()
    {
        if (stunCamera == null) return;

        Plane[] plane = GeometryUtility.CalculateFrustumPlanes(stunCamera);

        foreach (var mono in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if(mono is IStunnable stunnable)
            {
                Collider col = mono.GetComponentInChildren<Collider>();
                if (col == null) continue;

                if(GeometryUtility.TestPlanesAABB(plane, col.bounds))
                {
                    stunnable.OnStunned(null);
                }
            }
        }
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();

        if(flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        ui.ShowCameraFlash(false);
        ui.ShowStunCameraEffect(false);
    }
}
