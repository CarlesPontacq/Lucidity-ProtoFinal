using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private SceneController controller;

    [SerializeField] private GameObject mainMenuUX;
    [SerializeField] private GameObject optionsMenuUX;
    [SerializeField] private GameObject controlsMenuUX;
    [SerializeField] private GameObject extrasMenuUX;
    [SerializeField] private Button continueButton;

    void Start()
    {
        controller = SceneController.Instance;
        mainMenuUX.SetActive(true);
        optionsMenuUX.SetActive(false);
        extrasMenuUX.SetActive(false);
        ActivateContinueButton();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {

    }

    void ActivateContinueButton()
    {
        if (controller.ThereIsSavedACheckpoint())
            continueButton.interactable = true;
        else
            continueButton.interactable = false;
    }

    public void OnPlayButtonClick()
    {
        controller.LoadNextScene();
    }

    public void OnContinueButtonClick()
    {
        controller.LoadCheckpointScene();
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

    public void OnControlsButtonClick()
    {
        controlsMenuUX.SetActive(true);
        mainMenuUX.SetActive(false);
    }

    public void OnExtrasButtonClick()
    {
        extrasMenuUX.SetActive(true);
        mainMenuUX.SetActive(false);
    }

    public void OnCloseOptionsClick()
    {
        mainMenuUX.SetActive(true);
        optionsMenuUX.SetActive(false);
    }

    public void OnCloseControlsClick()
    {
        mainMenuUX.SetActive(true);
        controlsMenuUX.SetActive(false);
    }

    public void OnCloseExtrasClick()
    {
        mainMenuUX.SetActive(true);
        extrasMenuUX.SetActive(false);
    }
}
