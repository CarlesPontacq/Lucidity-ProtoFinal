using System;
using System.Collections;
using UnityEngine;

public class CameraFunctionality : MonoBehaviour
{
    [Header("General")]
    public bool isUnlocked;
    public bool isActive { get; private set; }

    public bool isPerformingAction = false;

    [SerializeField] protected CameraUIHandler ui;
    [SerializeField] protected CameraAudioHandler audioHandler;

    [Header("Observation Function")]
    [SerializeField] private Camera normalCamera;

    [Header("Stun Function")]
    public bool hasFlashCamera = false;
    public int maxReels = 5;
    public int currentReels;
    [SerializeField] private float flashDuration = 0.3f;
    private Coroutine flashCoroutine;
    [SerializeField] private Camera stunCamera;
    [SerializeField] private CameraRotation cameraRotation;

    [Header("Post-Process")]
    [SerializeField] protected GameObject globalVolumeMode;

    void Start()
    {
        ui = FindAnyObjectByType<CameraUIHandler>();
        audioHandler = FindAnyObjectByType<CameraAudioHandler>();

        currentReels = maxReels;
    }

    private void Update()
    {
        EnemyIsInSight();
    }

    public void ActivateMode()
    {
        isActive = true;
        globalVolumeMode.SetActive(true);

        normalCamera.enabled = false;
    }

    public void DeactivateMode()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        isActive = false;
        globalVolumeMode.SetActive(false);

        normalCamera.enabled = true;

        ui.ShowCameraFlash(false);
    }

    public void PerformCameraPhoto()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        if(currentReels > 0)
        {
            isPerformingAction = true;

            currentReels--;

            ui.ShowCameraFlash(true);
            flashCoroutine = StartCoroutine(StunFlashCoroutine());

            if (ui != null)
                ui.ActualizeRemainingReelsIndicator(currentReels);

            audioHandler.PlayPhotoSfx();
        }

    }

    private IEnumerator StunFlashCoroutine()
    {
        cameraRotation.SetControlEnabled(false);
        GameManager.Instance.SetPlayerControlEnabled(false);

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
            if (mono is IStunnable stunnable)
            {
                Collider col = mono.GetComponentInChildren<Collider>();
                if (col == null) continue;

                if (GeometryUtility.TestPlanesAABB(plane, col.bounds))
                {
                    stunnable.OnStunned(null);
                }
            }
        }
    }

    private void EnemyIsInSight()
    {
        if (stunCamera == null || !isActive) return;

        bool showRedLight = false;

        Plane[] plane = GeometryUtility.CalculateFrustumPlanes(normalCamera);

        foreach (var mono in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (mono is IStunnable stunnable)
            {
                Collider col = mono.GetComponentInChildren<Collider>();
                if (col == null) continue;

                if (GeometryUtility.TestPlanesAABB(plane, col.bounds))
                {
                    if (HasLineOfSight(col))
                    {
                        showRedLight = true;
                        break;
                    }
                }
            }
        }

        ui.ShowCameraRedLight(showRedLight);
    }

    private bool HasLineOfSight(Collider targetCollider)
    {
        Vector3 cameraPosition = normalCamera.transform.position;

        Vector3 targetPosition = targetCollider.bounds.center;

        Vector3 direction = targetPosition - cameraPosition;
        float distance = direction.magnitude;

        RaycastHit hit;

        if (Physics.Raycast(cameraPosition, direction, out hit, distance))
        {
            if (hit.collider == targetCollider ||
                hit.collider.transform.IsChildOf(targetCollider.transform) ||
                targetCollider.transform.IsChildOf(hit.collider.transform))
            {
                return true;
            }

            Debug.DrawLine(cameraPosition, hit.point, Color.red, 0.1f);
            return false;
        }

        return false;
    }

    public void ResetReels()
    {
        currentReels = maxReels;
        if (ui != null)
            ui.ActualizeRemainingReelsIndicator(currentReels);
    }
}
