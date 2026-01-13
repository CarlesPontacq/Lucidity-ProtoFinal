using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    private int currentScene;


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        currentScene = SceneManager.GetActiveScene().buildIndex;
        Debug.Log(currentScene);
    }


    void Update()
    {
        
    }

    public void LoadNextScene()
    {
        if (currentScene == SceneManager.sceneCount || currentScene < 0) return;

        currentScene = SceneManager.GetActiveScene().buildIndex;

        int nextScene = ++currentScene;
        SceneManager.LoadScene(nextScene);
    }

    public void LoadCreditsScene()
    {
        int creditsScene = SceneManager.sceneCountInBuildSettings - 1;

        SceneManager.LoadScene(creditsScene);
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }
}
