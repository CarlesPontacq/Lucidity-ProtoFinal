using UnityEngine;

public class RopeObject : ObjectInteraction
{
    [SerializeField] private TransitionToNextScene transition;

    public override void Interact()
    {
        if (transition != null)
            StartCoroutine(transition.PlayTransition());
        else
            SceneController.Instance.LoadNextScene();
    }
}