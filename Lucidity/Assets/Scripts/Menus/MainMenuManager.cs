using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private SceneController controller;

    void Start()
    {
        controller = SceneController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayButtonClick()
    {
        controller.LoadNextScene();
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }

    public void OnCreditsButtonClick()
    {
        controller.LoadCreditsScene();
    }
}
