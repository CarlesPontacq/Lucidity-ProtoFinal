using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class SceneTransition : MonoBehaviour
{
    [SerializeField] private CameraRotation cameraRotation;
    [SerializeField] private Image blackFadeImage;
    [SerializeField] protected float fadeDuration = 1f;

    public abstract IEnumerator PlayTransition();

    protected IEnumerator FadeToBlack(float duration)
    {
        float t = 0f;
        Color c = blackFadeImage.color;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;

            c.a = duration <= 0f ? 1f : Mathf.Clamp01(t / duration);

            blackFadeImage.color = c;

            yield return null;
        }

        c.a = 1f;
        blackFadeImage.color = c;
    }

    protected IEnumerator Wait(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
    }

    protected void DisablePlayer()
    {
        GameManager.Instance.SetPlayerControlEnabled(false);
        cameraRotation.SetControlEnabled(false);
    }

    protected void PlaySound(string id, float volume = 1f)
    {
        SFXManager.Instance.PlayGlobalSound(id, volume);
    }

    protected void LoadNextScene()
    {
        SceneController.Instance.LoadNextScene();
    }
}