using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionToAct3 : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image blackFadeImage;
    [SerializeField] private MonoBehaviour playerController;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float delayAfterBlack = 0.5f;
    [SerializeField] private float delayAfterSound = 1f;

    private void Awake()
    {
        SetupImage(blackFadeImage);
    }

    public IEnumerator PlayTransition()
    {
        if (playerController != null)
            playerController.enabled = false;

        if (blackFadeImage != null)
            yield return FadeToBlack(fadeDuration);

        yield return WaitRealtime(delayAfterBlack);

        SFXManager.Instance.PlayGlobalSound("shot", 1f);

        yield return WaitRealtime(delayAfterSound);

        SceneController.Instance.LoadNextScene();
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

    private IEnumerator WaitRealtime(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private void SetupImage(Image img)
    {
        if (img == null) return;

        img.gameObject.SetActive(true);
        img.raycastTarget = false;

        Color c = img.color;
        c.a = 0f;
        img.color = c;
    }
}