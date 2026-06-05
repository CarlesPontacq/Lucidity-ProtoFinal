using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class HangingCinematicTrigger : MonoBehaviour
{
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private TransitionToNextScene transition;
    [SerializeField] private GameObject cinematicCamera;
    private string playerTag = "Player";
    [SerializeField] private float waitTimeBeforeSounds = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == playerTag)
        {
            cinematicCamera.SetActive(true);
            GameManager.Instance.SetPlayerControlEnabled(false);
            GameManager.DeathHandlerRef.SetPlayerBodyVisible(false);
            gameplayUI.SetActive(false);
            StartCoroutine(StartTransition());
        }
    }

    private IEnumerator StartTransition()
    {
        yield return new WaitForSecondsRealtime(waitTimeBeforeSounds);

        if (transition != null)
            StartCoroutine(transition.PlayTransition());
        else
            SceneController.Instance.LoadNextScene();
    }
}
