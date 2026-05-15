using System.Collections;
using UnityEditor.EditorTools;
using UnityEngine;

public class CinematicHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DeathCameraEffect deathEffect;
    [SerializeField] private CameraRotation cameraRotation;
    [SerializeField] private LoopManager loopManager;

    [Header("Disable components")]
    [SerializeField] private MonoBehaviour[] disableOnDeath;
    [SerializeField] private GameObject playerBody;

    [Header("Death Related")]
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private float deathCooldown = 0.35f;
    public bool isDying = false;

    private float nextAllowedDeathTime = 0f;

    public void SetPlayerControlEnabled(bool enabled)
    {
        if (disableOnDeath == null) return;

        for (int i = 0; i < disableOnDeath.Length; i++)
            if (disableOnDeath[i] != null)
                disableOnDeath[i].enabled = enabled;
    }

    private void SetPlayerBodyVisible(bool visible)
    {
        if (playerBody == null) return;

        Renderer[] renderers = playerBody.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].enabled = visible;
    }

    #region Player Death
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

        SetPlayerControlEnabled(false);

        // Ocultar body
        SetPlayerBodyVisible(false);
        cameraRotation.SetControlEnabled(false);

        // Animación de muerte
        if (deathEffect != null)
            yield return deathEffect.PlayDeathSequence();
        else
            yield return new WaitForSecondsRealtime(5f);

        // Reset loops
        GameManager.Instance.SubtractLoopToCount();
        if (loopManager != null)
            loopManager.StartLoopFresh();

        yield return RespawnPlayer();

        if (deathEffect != null)
        {
            deathEffect.ClearOverlays();
            deathEffect.RestoreAfterRespawn();
        }

        SetPlayerBodyVisible(true);
        cameraRotation.SetControlEnabled(true);
        cameraRotation.ResetOrientation();

        SetPlayerControlEnabled(true);

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
    #endregion
}
