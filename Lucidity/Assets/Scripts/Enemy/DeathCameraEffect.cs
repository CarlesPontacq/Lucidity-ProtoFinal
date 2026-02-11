using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering; 

public class DeathCameraEffect : MonoBehaviour
{
    [Header("Pitch Target (lo que rota arriba/abajo)")]
    [SerializeField] private Transform pitchTarget;

    [Header("Duración del movimiento hacia arriba")]
    [SerializeField] private float lookUpDuration = 5f;

    [Header("Ángulo final (mirar al techo)")]
    [SerializeField] private float lookUpAngle = -80f;

    [Header("Post Process (B/N)")]
    [SerializeField] private Volume deathBWVolume; 

    [Header("UI (opcional)")]
    [SerializeField] private Image blackFadeImage;
    [SerializeField] private Image flashImage;

    [Header("Fade/Flash (opcional)")]
    [SerializeField] private float fadeToBlackDuration = 1.0f;
    [SerializeField] private float flashIn = 0.08f;
    [SerializeField] private float flashHold = 0.05f;
    [SerializeField] private float flashOut = 0.12f;

    private Quaternion preDeathLocalRot;

    private bool lockPitch = false;
    private Quaternion lockedLocalRot;

    private void Awake()
    {
        if (pitchTarget == null)
            pitchTarget = transform;

        preDeathLocalRot = pitchTarget.localRotation;

        SetupImage(blackFadeImage);
        SetupImage(flashImage);

        if (deathBWVolume != null)
            deathBWVolume.enabled = false;
    }

    private void LateUpdate()
    {
        if (lockPitch && pitchTarget != null)
            pitchTarget.localRotation = lockedLocalRot;
    }

    private void SetupImage(Image img)
    {
        if (img == null) return;

        img.gameObject.SetActive(true);
        img.raycastTarget = false;

        var c = img.color;
        c.a = 0f;
        img.color = c;
    }

    public IEnumerator PlayDeathSequence()
    {
        if (pitchTarget == null) yield break;

        preDeathLocalRot = pitchTarget.localRotation;

        lockPitch = true;
        lockedLocalRot = pitchTarget.localRotation;

        if (deathBWVolume != null)
            deathBWVolume.enabled = true;

        if (flashImage != null)
            yield return FlashRoutine();

        yield return LookUpRoutine(lookUpDuration);

        if (blackFadeImage != null)
            yield return FadeToBlack(fadeToBlackDuration);
    }


    private IEnumerator LookUpRoutine(float duration)
    {
        Vector3 startEuler = pitchTarget.localEulerAngles;
        float startPitch = NormalizePitch(startEuler.x);
        float targetPitch = lookUpAngle;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = duration <= 0f ? 1f : Mathf.Clamp01(t / duration);

            float pitch = Mathf.Lerp(startPitch, targetPitch, a);

            Vector3 e = pitchTarget.localEulerAngles;
            e.x = pitch;

            Quaternion rot = Quaternion.Euler(e);

            lockedLocalRot = rot;
            pitchTarget.localRotation = rot;

            yield return null;
        }

        Vector3 endEuler = pitchTarget.localEulerAngles;
        endEuler.x = targetPitch;

        lockedLocalRot = Quaternion.Euler(endEuler);
        pitchTarget.localRotation = lockedLocalRot;
    }

    private IEnumerator FadeToBlack(float duration)
    {
        float t = 0f;
        Color c = blackFadeImage.color;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = duration <= 0f ? 1f : Mathf.Clamp01(t / duration);
            c.a = a;
            blackFadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        blackFadeImage.color = c;
    }

    private IEnumerator FlashRoutine()
    {
        flashImage.transform.SetAsLastSibling();

        yield return FadeAlpha(flashImage, 0f, 1f, flashIn);
        yield return WaitRealtime(flashHold);
        yield return FadeAlpha(flashImage, 1f, 0f, flashOut);
    }

    private IEnumerator FadeAlpha(Image img, float from, float to, float duration)
    {
        float t = 0f;
        Color c = img.color;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = duration <= 0f ? 1f : Mathf.Clamp01(t / duration);
            c.a = Mathf.Lerp(from, to, a);
            img.color = c;
            yield return null;
        }

        c.a = to;
        img.color = c;
    }

    private IEnumerator WaitRealtime(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    public void ClearOverlays()
    {
        ResetImage(blackFadeImage);
        ResetImage(flashImage);
    }

    public void RestoreAfterRespawn()
    {
        lockPitch = false;

        if (pitchTarget != null)
            pitchTarget.localRotation = preDeathLocalRot;

        lockedLocalRot = preDeathLocalRot;

        if (deathBWVolume != null)
            deathBWVolume.enabled = false;
    }


    private void ResetImage(Image img)
    {
        if (img == null) return;
        var c = img.color;
        c.a = 0f;
        img.color = c;
    }

    private float NormalizePitch(float x)
    {
        if (x > 180f) x -= 360f;
        return x;
    }
}
