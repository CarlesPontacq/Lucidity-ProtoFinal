using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private SceneController controller;

    [SerializeField] private GameObject mainMenuUX;
    [SerializeField] private GameObject optionsMenuUX;

    void Start()
    {
        controller = SceneController.Instance;
        mainMenuUX.SetActive(true);
        optionsMenuUX.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

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

    public void OnOptionsButtonClick()
    {
        optionsMenuUX.SetActive(true);
        mainMenuUX.SetActive(false);
    }

    public void OnCloseOptionsClick()
    {
        mainMenuUX.SetActive(true);
        optionsMenuUX.SetActive(false);
    }
}
