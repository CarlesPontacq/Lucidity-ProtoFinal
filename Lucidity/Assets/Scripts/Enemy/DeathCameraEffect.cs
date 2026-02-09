using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DeathCameraEffect : MonoBehaviour
{
    [Header("Transform que rota (pitch)")]
    [SerializeField] private Transform pitchTarget;

    [Header("UI Flash Blanco")]
    [SerializeField] private Image flashImage;

    [Header("UI Fade Negro")]
    [SerializeField] private Image blackFadeImage;

    [Header("Post Process")]
    [SerializeField] private Volume globalVolume;

    [Header("Look Up")]
    [SerializeField] private float lookUpAngle = -80f;
    [SerializeField] private float lookUpDuration = 0.6f;

    [Header("Fade Black")]
    [SerializeField] private float fadeToBlackDuration = 0.6f;

    [Header("Flash Timing")]
    [SerializeField] private float flashIn = 0.1f;
    [SerializeField] private float flashHold = 0.05f;
    [SerializeField] private float flashOut = 0.15f;

    private ColorAdjustments colorAdjustments;
    private Quaternion originalPitchRot;

    private void Awake()
    {
        if (pitchTarget == null)
            pitchTarget = transform;

        originalPitchRot = pitchTarget.localRotation;

        SetupImage(flashImage);
        SetupImage(blackFadeImage);

        if (globalVolume != null && globalVolume.profile != null)
            globalVolume.profile.TryGet(out colorAdjustments);
    }

    private void SetupImage(Image img)
    {
        if (img == null) return;
        var c = img.color;
        c.a = 0f;
        img.color = c;
        img.gameObject.SetActive(true);
        img.raycastTarget = false;
    }

    public IEnumerator PlayDeathSequence()
    {
        originalPitchRot = pitchTarget.localRotation;

        SetBlackAndWhite(true);

        yield return LookUpRoutine();
        yield return FadeToBlack();
        yield return FlashRoutine();
    }

    public void Restore()
    {
        pitchTarget.localRotation = originalPitchRot;
        SetBlackAndWhite(false);

        ResetImage(flashImage);
        ResetImage(blackFadeImage);
    }

    private void ResetImage(Image img)
    {
        if (img == null) return;
        var c = img.color;
        c.a = 0f;
        img.color = c;
    }

    private void SetBlackAndWhite(bool enabled)
    {
        if (colorAdjustments == null) return;

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

    private IEnumerator FadeToBlack()
    {
        if (blackFadeImage == null) yield break;

        float t = 0f;
        Color c = blackFadeImage.color;

        while (t < fadeToBlackDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / fadeToBlackDuration);
            c.a = a;
            blackFadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        blackFadeImage.color = c;
    }

    private IEnumerator FlashRoutine()
    {
        if (flashImage == null) yield break;

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

    private float NormalizePitch(float x)
    {
        if (x > 180f) x -= 360f;
        return x;
    }
}
