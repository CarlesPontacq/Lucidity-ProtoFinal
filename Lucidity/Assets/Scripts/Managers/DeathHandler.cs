using System.Collections;
using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DeathCameraEffect deathEffect;
    [SerializeField] private CameraRotation cameraRotation;
    [SerializeField] private GameObject playerBody;

    [Header("Death Related")]
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private float deathCooldown = 0.35f;
    public bool isDying = false;

    private float nextAllowedDeathTime = 0f;

    public void SetPlayerBodyVisible(bool visible)
    {
        if (playerBody == null) return;

        Renderer[] renderers = playerBody.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].enabled = visible;
    }

    public void PlayerDied()
    {
        if (CheatsManager.Instance != null && CheatsManager.Instance.currentlyImmortal) return;
        if (isDying) return;
        if (Time.time < nextAllowedDeathTime) return;

        nextAllowedDeathTime = Time.time + deathCooldown;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        isDying = true;

        Time.timeScale = 1f;

        GameManager.Instance.SetPlayerControlEnabled(false);

        // Ocultar body
        SetPlayerBodyVisible(false);
        cameraRotation.SetControlEnabled(false);

        // Animación de muerte
        if (deathEffect != null)
            yield return deathEffect.PlayDeathSequence();
        else
            yield return new WaitForSecondsRealtime(5f);

        // Reset loops
        if (GameManager.LoopManagerRef != null)
        {
            GameManager.LoopManagerRef.SubtractLoopToCount();
            GameManager.LoopManagerRef.StartLoopFresh();
        }

        yield return RespawnPlayer();

        if (deathEffect != null)
        {
            deathEffect.ClearOverlays();
            deathEffect.RestoreAfterRespawn();
        }

        SetPlayerBodyVisible(true);
        cameraRotation.SetControlEnabled(true);
        cameraRotation.ResetOrientation();

        GameManager.Instance.SetPlayerControlEnabled(true);

        var deathTouch = GameManager.PlayerRef.GetComponentInChildren<PlayerDeathOnEnemyTouch>(true);
        if (deathTouch != null)
            deathTouch.ResetDeathTrigger();

        isDying = false;
    }

    private IEnumerator RespawnPlayer()
    {
        if (GameManager.PlayerRef == null || playerSpawn == null) yield break;

        var cols = GameManager.PlayerRef.GetComponentsInChildren<Collider>(true);
        foreach (var col in cols)
            col.enabled = true;

        Rigidbody rb = GameManager.PlayerRef.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.position = playerSpawn.position;
            rb.rotation = playerSpawn.rotation;

            rb.WakeUp();
        }
        else
        {
            GameManager.PlayerRef.transform.SetPositionAndRotation(playerSpawn.position, playerSpawn.rotation);
        }

        Physics.SyncTransforms();

        yield return null;
    }
}
