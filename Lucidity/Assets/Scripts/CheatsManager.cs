using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatsManager : MonoBehaviour
{
    public static CheatsManager Instance { get; private set; }

    [SerializeField] ReportSheetOverlayUI reportSheetScript;

    [SerializeField] KeyCode nextSceneKey = KeyCode.N;
    [SerializeField] KeyCode openDoorKey = KeyCode.O;
    [SerializeField] KeyCode immortalityKey = KeyCode.I;

    public bool currentlyImmortal = false;

    private void Awake()
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

    private void Update()
    {
        if (Input.GetKeyDown(nextSceneKey))
            SceneController.Instance.LoadNextScene();

        if (Input.GetKeyDown(openDoorKey))
            reportSheetScript.UnlockNextLoop(true);

        if (Input.GetKeyDown(immortalityKey))
            currentlyImmortal = !currentlyImmortal;

        if (Input.GetKeyDown(KeyCode.Alpha4))
            GameManager.Instance.SetCurrentLoopIndex(3);
        if (Input.GetKeyDown(KeyCode.Alpha8))
            GameManager.Instance.SetCurrentLoopIndex(7);    
    }
}
