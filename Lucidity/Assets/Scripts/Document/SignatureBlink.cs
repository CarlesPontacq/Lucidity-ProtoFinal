using UnityEngine;
using UnityEngine.UI;

public class SignatureBlink : MonoBehaviour
{
    [SerializeField] private Image targetImage;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 1.2f;   
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 1f;

    [Header("Curve")]
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float time;
    private bool goingUp = true;

    private void Reset()
    {
        targetImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (targetImage == null) return;

        time += Time.unscaledDeltaTime;

        float t = Mathf.Clamp01(time / fadeDuration);
        float curveValue = fadeCurve.Evaluate(t);

        float alpha;

        if (goingUp)
            alpha = Mathf.Lerp(minAlpha, maxAlpha, curveValue);
        else
            alpha = Mathf.Lerp(maxAlpha, minAlpha, curveValue);

        SetAlpha(alpha);

        if (time >= fadeDuration)
        {
            time = 0f;
            goingUp = !goingUp;
        }
    }

    private void SetAlpha(float a)
    {
        Color c = targetImage.color;
        c.a = a;
        targetImage.color = c;
    }
}