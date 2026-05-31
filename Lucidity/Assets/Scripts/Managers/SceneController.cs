using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    private int currentScene;
    private int mainMenuScene = 1;
    private int creditsScene = 6;

    private int currentCheckpointAct;
    private const string CURRENT_ACT = "current_act";

    private int currentMaxCompletedAct;
    private const string COMPLETED_ACTS = "completed_acts";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentCheckpointAct = PlayerPrefs.GetInt(CURRENT_ACT, 0);
        SaveCheckpoint();
    }

    void SaveCompletedAct()
    {
        if (currentScene > mainMenuScene && currentScene < creditsScene)
        {
            int completedActs = currentScene - 2;
            if (completedActs > PlayerPrefs.GetInt(COMPLETED_ACTS))
            {
                PlayerPrefs.SetInt(COMPLETED_ACTS, completedActs);
                currentMaxCompletedAct = completedActs;
            }          
        }
    }

    void SaveCheckpoint()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;

        if(currentScene > mainMenuScene && currentScene < creditsScene)
        {
            PlayerPrefs.SetInt(CURRENT_ACT, currentScene);
            currentCheckpointAct = PlayerPrefs.GetInt(CURRENT_ACT);
        }
    }

    public bool ThereIsSavedACheckpoint()
    {
        return currentCheckpointAct > currentScene;
    }

    public void LoadNextScene()
    {
        if (currentScene >= SceneManager.sceneCountInBuildSettings || currentScene < 0) return;

        currentMaxCompletedAct = PlayerPrefs.GetInt(COMPLETED_ACTS, 0);
        SaveCompletedAct();

        int nextScene = ++currentScene;
        SceneManager.LoadScene(nextScene);
    }

    public void LoadCheckpointScene()
    {
        if (currentCheckpointAct >= SceneManager.sceneCountInBuildSettings || currentCheckpointAct < 0) return;

        SceneManager.LoadScene(currentCheckpointAct);
    }

    public void LoadPrevScene()
    {
        Debug.Log(SceneManager.sceneCountInBuildSettings);
        if (currentScene > SceneManager.sceneCountInBuildSettings || currentScene <= 0) return;

        int nextScene = --currentScene;
        SceneManager.LoadScene(nextScene);
    }

    public void LoadCreditsScene()
    {
        int creditsScene = SceneManager.sceneCountInBuildSettings - 1;

        SceneManager.LoadScene(creditsScene);
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
