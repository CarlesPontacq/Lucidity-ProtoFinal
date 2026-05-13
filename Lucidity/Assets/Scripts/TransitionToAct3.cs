using System.Collections;
using UnityEngine;

public class TransitionToAct3 : SceneTransition
{
    [SerializeField] private float delayAfterBlack = 0.5f;
    [SerializeField] private float delayAfterSound = 1f;

    public override IEnumerator PlayTransition()
    {
        DisablePlayer();

        yield return FadeToBlack(fadeDuration);

        yield return Wait(delayAfterBlack);

        PlaySound("shot");

        yield return Wait(delayAfterSound);

        LoadNextScene();
    }
}