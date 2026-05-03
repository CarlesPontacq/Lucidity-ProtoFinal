using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionToCredits : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image blackFadeImage;
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private CameraRotation cameraRotation;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 1f;

    [Header("Delays")]
    [SerializeField] private float delayAfterBlack = 0.5f;
    [SerializeField] private float delayAfterFirstSound = 1f;
    [SerializeField] private float delayAfterSecondSound = 1f;

    private void Awake()
    {
        SetupImage(blackFadeImage);
    }

    public IEnumerator PlayTransition()
    {
        GameManager.Instance.SetPlayerControlEnabled(false);
        cameraRotation.SetControlEnabled(false);

        if (playerController != null)
            playerController.enabled = false;

        if (blackFadeImage != null)
            yield return FadeToBlack(fadeDuration);

        yield return WaitRealtime(delayAfterBlack);

        SFXManager.Instance.PlayGlobalSound("chair", 0.3f);

        yield return WaitRealtime(delayAfterFirstSound);

        SFXManager.Instance.PlayGlobalSound("rope", 1f);

        yield return WaitRealtime(delayAfterSecondSound);

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