using System.Collections;
using UnityEngine;

public class TransitionToNextScene : SceneTransition
{
    [SerializeField] private float delayAfterBlack = 0.5f;
    [SerializeField] private float delayAfterSound = 1f;
    [SerializeField] private string soundName;

    public override IEnumerator PlayTransition()
    {
        DisablePlayer();

        yield return FadeToBlack(fadeDuration);

        yield return Wait(delayAfterBlack);

        yield return SFXManager.Instance.PlayGlobalSoundAndWait(soundName);

        yield return Wait(delayAfterSound);

        LoadNextScene();
    }
}
