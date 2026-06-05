using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CinematicSkipPanelUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image fillImage;

    [SerializeField] private float fadeSpeed = 8f;
    [SerializeField] private float snapThreshold = 0.01f;

    private Coroutine fadeRoutine;

    public void SetProgress(float value)
    {
        fillImage.fillAmount = value;
    }

    public void Show()
    {
        StartFade(1f);
    }

    public void Hide()
    {
        StartFade(0f);
    }

    private void StartFade(float target)
    {
        if (Mathf.Abs(canvasGroup.alpha - target) < snapThreshold)
        {
            canvasGroup.alpha = target;
            return;
        }

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeTo(target));
    }

    private IEnumerator FadeTo(float target)
    {
        float start = canvasGroup.alpha;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeSpeed;

            float value = Mathf.Lerp(start, target, t);

            if (Mathf.Abs(value - target) < snapThreshold)
            {
                canvasGroup.alpha = target;
                yield break;
            }

            canvasGroup.alpha = value;

            yield return null;
        }

        canvasGroup.alpha = target;
    }
}