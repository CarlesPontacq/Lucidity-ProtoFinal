using System.Collections;
using UnityEngine;

public class TransitionToCredits : SceneTransition
{

    [Header("Delays")]
    [SerializeField] private float delayAfterBlack = 0.5f;
    [SerializeField] private float delayAfterFirstSound = 1f;
    [SerializeField] private float delayAfterSecondSound = 1f;

    public override IEnumerator PlayTransition()
    {
        DisablePlayer();

        yield return FadeToBlack(fadeDuration);

        yield return Wait(delayAfterBlack);

        PlaySound("chair", 0.3f);

        yield return Wait(delayAfterFirstSound);

        PlaySound("rope");

        yield return Wait(delayAfterSecondSound);

        LoadNextScene();
    }
}