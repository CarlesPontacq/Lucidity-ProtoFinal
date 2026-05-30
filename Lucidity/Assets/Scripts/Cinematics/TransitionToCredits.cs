using System.Collections;
using UnityEngine;

public class TransitionToCredits : SceneTransition
{

    [Header("Delays")]
    [SerializeField] private float delayAfterBlack = 0.5f;
    [SerializeField] private float delayAfterSound = 1f;
    [SerializeField] private string soundName;

    public override IEnumerator PlayTransition()
    {
        yield return SFXManager.Instance.PlayGlobalSoundAndWait(soundName);

        yield return Wait(delayAfterSound);

        yield return FadeToBlack(fadeDuration);

        yield return Wait(delayAfterBlack);

        LoadNextScene();
    }
}