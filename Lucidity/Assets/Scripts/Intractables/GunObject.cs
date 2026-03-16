using UnityEngine;

public class GunObject : ObjectInteraction
{
    public override void Interact()
    {
        GameManager.Instance.GunGrabbed();
        Destroy(gameObject);
        SceneController.Instance.LoadNextScene();
    }
}
