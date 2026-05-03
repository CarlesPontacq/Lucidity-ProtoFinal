using UnityEngine;

public class RopeObject : ObjectInteraction
{
    [SerializeField] private TransitionToCredits transition;

    public override void Interact()
    {
        if (transition != null)
            StartCoroutine(transition.PlayTransition());
        else
            SceneController.Instance.LoadNextScene();
    }
}