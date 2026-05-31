using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CinematicFade : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;

    private Coroutine routine;

    private void Awake()
    {
        SetAlpha(0f);
    }

    public void FadeToBlack(Action onComplete)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(Fade(1f, onComplete));
    }

    private IEnumerator Fade(float target, Action onComplete)
    {
        float start = fadeImage.color.a;
        float t = 0f;

        if (fadeDuration <= 0f)
        {
            SetAlpha(target);
            onComplete?.Invoke();
            yield break;
        }

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;

            float normalized = t / fadeDuration;
            float a = Mathf.Lerp(start, target, normalized);

            SetAlpha(a);

            yield return null;
        }

        SetAlpha(target);
        onComplete?.Invoke();
    }

    private void SetAlpha(float alpha)
    {
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
}