using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DeathCameraEffect : MonoBehaviour
{
    [Header("Which transform rotates up? (assign your camera OR vertical pivot)")]
    [SerializeField] private Transform pitchTarget;

    [Header("Flash UI")]
    [SerializeField] private Image flashImage;

    [Header("Black & White (URP Volume)")]
    [SerializeField] private Volume globalVolume;

    [Header("Look Up")]
    [SerializeField] private float lookUpAngle = -80f;
    [SerializeField] private float lookUpDuration = 0.45f;

    [Header("Flash Timing")]
    [SerializeField] private float flashIn = 0.08f;
    [SerializeField] private float flashHold = 0.06f;
    [SerializeField] private float flashOut = 0.14f;

    private ColorAdjustments colorAdjustments;
    private Quaternion originalPitchLocalRot;

    private void Awake()
    {
        if (pitchTarget == null)
            pitchTarget = transform; // fallback

        originalPitchLocalRot = pitchTarget.localRotation;

        if (flashImage != null)
        {
            var c = flashImage.color;
            c.a = 0f;
            flashImage.color = c;
            flashImage.gameObject.SetActive(true);
            flashImage.raycastTarget = false;
        }

        if (globalVolume != null && globalVolume.profile != null)
            globalVolume.profile.TryGet(out colorAdjustments);
    }

    public IEnumerator PlayDeathSequence()
    {
        // Guardar rot inicial del target real
        originalPitchLocalRot = pitchTarget.localRotation;

        Debug.Log($"[DeathFX] PlayDeathSequence pitchTarget={pitchTarget.name}");

        // BN ON
        SetBlackAndWhite(true);

        // mirar al techo
        yield return LookUpRoutine();

        // flash
        yield return FlashRoutine();
    }

    public void Restore()
    {
        pitchTarget.localRotation = originalPitchLocalRot;
        SetBlackAndWhite(false);

        if (flashImage != null)
        {
            var c = flashImage.color;
            c.a = 0f;
            flashImage.color = c;
        }
    }

    private void SetBlackAndWhite(bool enabled)
    {
        if (colorAdjustments == null) return;

        // URP suele ser -100..100
        colorAdjustments.saturation.overrideState = true;
        colorAdjustments.saturation.value = enabled ? -100f : 0f;
    }

    private IEnumerator LookUpRoutine()
    {
        Quaternion start = pitchTarget.localRotation;
        Vector3 e = start.eulerAngles;

        float startPitch = NormalizePitch(e.x);
        float targetPitch = lookUpAngle;

        float t = 0f;
        while (t < lookUpDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / lookUpDuration);
            float pitch = Mathf.Lerp(startPitch, targetPitch, a);

            pitchTarget.localRotation = Quaternion.Euler(pitch, e.y, e.z);
            yield return null;
        }

        pitchTarget.localRotation = Quaternion.Euler(targetPitch, e.y, e.z);
    }

    private IEnumerator FlashRoutine()
    {
        if (flashImage == null) yield break;

        yield return FadeAlpha(0f, 1f, flashIn);
        yield return WaitRealtime(flashHold);
        yield return FadeAlpha(1f, 0f, flashOut);
    }

    private IEnumerator FadeAlpha(float from, float to, float duration)
    {
        float t = 0f;
        Color c = flashImage.color;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = duration <= 0f ? 1f : Mathf.Clamp01(t / duration);
            c.a = Mathf.Lerp(from, to, a);
            flashImage.color = c;
            yield return null;
        }

        c.a = to;
        flashImage.color = c;
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

    private float NormalizePitch(float x)
    {
        if (x > 180f) x -= 360f;
        return x;
    }
}
