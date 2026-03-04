using UnityEngine;

public class LoopEnder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.finishedLoops)
                SceneController.Instance.LoadNextScene();
        }
    }
}
