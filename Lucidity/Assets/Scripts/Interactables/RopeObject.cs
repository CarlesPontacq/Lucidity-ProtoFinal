using UnityEngine;

public class RopeObject : ObjectInteraction
{
    public override void Interact()
    {
        SceneController.Instance.LoadNextScene();
    }
}
