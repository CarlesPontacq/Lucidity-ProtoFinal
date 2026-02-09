using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeathCameraEffect : MonoBehaviour
{
    [Header("Flash UI")]
    [SerializeField] private Image flashImage;

    [Header("Look Up")]
    [SerializeField] private float lookUpAngle = -80f;          
    [SerializeField] private float lookUpDuration = 0.45f;

    [Header("Flash Timing")]
    [SerializeField] private float flashIn = 0.08f;
    [SerializeField] private float flashHold = 0.06f;
    [SerializeField] private float flashOut = 0.14f;

    private Quaternion originalLocalRot;

    private void Awake()
    {
        originalLocalRot = transform.localRotation;

        if (flashImage != null)
        {
            var c = flashImage.color;
            c.a = 0f;
            flashImage.color = c;
            flashImage.gameObject.SetActive(true);
            flashImage.raycastTarget = false;
        }
    }

    public IEnumerator PlayDeathSequence()
    {
        originalLocalRot = transform.localRotation;

        yield return LookUpRoutine();

        yield return FlashRoutine();
    }

    public void Restore()
    {
        transform.localRotation = originalLocalRot;

        if (flashImage != null)
        {
            var c = flashImage.color;
            c.a = 0f;
            flashImage.color = c;
        }
    }

    private IEnumerator LookUpRoutine()
    {
        Quaternion start = transform.localRotation;

        Vector3 e = start.eulerAngles;
        float startPitch = NormalizePitch(e.x);
        float targetPitch = lookUpAngle;

        float t = 0f;
        while (t < lookUpDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / lookUpDuration);
            float pitch = Mathf.Lerp(startPitch, targetPitch, a);

            transform.localRotation = Quaternion.Euler(pitch, e.y, e.z);
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(targetPitch, e.y, e.z);
    }

    private IEnumerator FlashRoutine()
    {
        if (flashImage == null) yield break;

        // fade in
        yield return FadeAlpha(0f, 1f, flashIn);
        // hold
        yield return WaitRealtime(flashHold);
        // fade out
        yield return FadeAlpha(1f, 0f, flashOut);
    }

    private IEnumerator FadeAlpha(float from, float to, float duration)
    {
        if (flashImage == null) yield break;

        float t = 0f;
        Color c = flashImage.color;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = (duration <= 0f) ? 1f : Mathf.Clamp01(t / duration);
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
