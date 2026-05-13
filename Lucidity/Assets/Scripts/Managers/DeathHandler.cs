using System.Collections;
using UnityEditor.EditorTools;
using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DeathCameraEffect deathEffect;
    [SerializeField] private CameraRotation cameraRotation;
    [SerializeField] private LoopManager loopManager;

    [Header("Disable components")]
    [SerializeField] private MonoBehaviour[] disableOnDeath;
    [SerializeField] private GameObject playerBody;

    [Header("Spawn")]
    [SerializeField] private string playerSpawnTag = "PlayerSpawn";

    [Header("Death Safety")]
    [SerializeField] private float deathCooldown = 0.35f;

    [Header("Physics Safety")]
    [Tooltip("Layer que debe tener el Player root tras respawn (opcional). Déjalo en -1 para no tocar layer.")]

    public bool isDying = false;
    private float nextAllowedDeathTime = 0f;

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

        yield return TeleportAndRearmPhysics();

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

    private IEnumerator TeleportAndRearmPhysics()
    {
        if (GameManager.PlayerRef == null) yield break;

        GameObject sp = GameObject.FindGameObjectWithTag(playerSpawnTag);
        if (sp == null)
        {
            Debug.LogWarning($"No hay objeto con tag {playerSpawnTag}.");
            yield break;
        }

        Transform spawn = sp.transform;

        var cols = GameManager.PlayerRef.GetComponentsInChildren<Collider>(true);
        foreach (var col in cols)
            col.enabled = true;

        Rigidbody rb = GameManager.PlayerRef.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.position = spawn.position;
            rb.rotation = spawn.rotation;

            rb.WakeUp();
        }
        else
        {
            GameManager.PlayerRef.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
        }

        Physics.SyncTransforms();

        yield return null;
    }
}
