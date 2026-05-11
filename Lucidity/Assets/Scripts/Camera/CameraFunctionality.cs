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
    [SerializeField] float lineOfSightRadius = 0.2f;
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
                    if (HasLineOfSight(col))
                    {
                        stunnable.OnStunned(null);
                        break;
                    }
                }
            }
        }
    }

    private bool HasLineOfSight(Collider targetCollider)
    {
        Vector3 cameraPosition = normalCamera.transform.position;
        Vector3 targetPosition = targetCollider.bounds.center;
        Vector3 direction = targetPosition - cameraPosition;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.SphereCastAll(cameraPosition, lineOfSightRadius, direction, distance);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == targetCollider ||
                hit.collider.transform.IsChildOf(targetCollider.transform) ||
                targetCollider.transform.IsChildOf(hit.collider.transform))
            {
                Debug.DrawLine(cameraPosition, hit.point, Color.green, 0.1f);
                return true;
            }
        }

        return false;
    }

    public void ResetReels()
    {
        currentReels = maxReels;
        if (ui != null)
            ui.ActualizeRemainingReelsIndicator(currentReels);
    }

    private void OnDrawGizmos()
    {
        if (normalCamera == null) return;

        // Optional: Only draw when in Play Mode
        if (!Application.isPlaying) return;

        // Find a specific target or use a configurable one
        // For demonstration, this will draw for all stunnable objects

        var stunnableObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (var mono in stunnableObjects)
        {
            if (mono is IStunnable stunnable)
            {
                Collider col = mono.GetComponentInChildren<Collider>();
                if (col == null) continue;

                Vector3 cameraPosition = normalCamera.transform.position;
                Vector3 targetPosition = col.bounds.center;
                Vector3 direction = targetPosition - cameraPosition;
                float distance = direction.magnitude;

                // Draw the main line
                Gizmos.color = Color.white;
                Gizmos.DrawLine(cameraPosition, targetPosition);

                // Draw the sphere cast volume (as multiple lines)
                Vector3 dirNorm = direction.normalized;
                Vector3 right = Vector3.Cross(dirNorm, Vector3.up).normalized;
                Vector3 up = Vector3.Cross(right, dirNorm);

                Gizmos.color = new Color(0.5f, 0.5f, 1, 0.5f);

                // Draw 8 lines around the capsule
                for (int i = 0; i < 8; i++)
                {
                    float angle = i * 45 * Mathf.Deg2Rad;
                    Vector3 offset = (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * lineOfSightRadius;
                    Gizmos.DrawLine(cameraPosition + offset, targetPosition + offset);
                }

                // Draw end caps
                Gizmos.DrawWireSphere(cameraPosition, lineOfSightRadius);
                Gizmos.DrawWireSphere(targetPosition, lineOfSightRadius);
            }
        }
    }
}
