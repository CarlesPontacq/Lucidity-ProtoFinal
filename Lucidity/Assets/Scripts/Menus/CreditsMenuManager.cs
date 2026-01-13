using UnityEngine;

public class CreditsMenuManager : MonoBehaviour
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

    public void OnReturnButtonClick()
    {
        controller.LoadMainMenuScene(); 
    }
}
