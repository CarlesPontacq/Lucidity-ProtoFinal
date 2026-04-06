using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatsManager : MonoBehaviour
{
    public static CheatsManager Instance { get; private set; }

    ReportSheetOverlayUI reportSheetScript;

    [SerializeField] KeyCode nextSceneKey = KeyCode.N;
    [SerializeField] KeyCode prevSceneKey = KeyCode.P;
    [SerializeField] KeyCode openDoorKey = KeyCode.O;
    [SerializeField] KeyCode immortalityKey = KeyCode.I;
    [SerializeField] KeyCode nextLoopKey = KeyCode.L;
    [SerializeField] KeyCode restartLoopsKey = KeyCode.R;
    [SerializeField] KeyCode unlockDoorsKey = KeyCode.Alpha4;
    [SerializeField] KeyCode goToLastLoopKey = KeyCode.Alpha8;

    public bool currentlyImmortal = false;

    [SerializeField] private int lastLoop = 4;

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

        if(reportSheetScript == null)
            reportSheetScript = FindAnyObjectByType<ReportSheetOverlayUI>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(nextSceneKey))
            SceneController.Instance.LoadNextScene();

        if (Input.GetKeyDown(prevSceneKey))
            SceneController.Instance.LoadPrevScene();

        if(reportSheetScript != null)
        {
            if (Input.GetKeyDown(nextLoopKey))
            {
                reportSheetScript.UnlockNextLoop(true);
                GameManager.Instance.OnExitDoorCrossed();

            }

            if (Input.GetKeyDown(restartLoopsKey))
            {
                reportSheetScript.UnlockNextLoop(false);
                GameManager.Instance.OnExitDoorCrossed();
                GameManager.Instance.ResetLoops();
            }

            if (Input.GetKeyDown(openDoorKey))
                reportSheetScript.UnlockNextLoop(true);
        }

        if (Input.GetKeyDown(immortalityKey))
            currentlyImmortal = !currentlyImmortal;

        if (Input.GetKeyDown(goToLastLoopKey))
            GameManager.Instance.SetCurrentLoopIndex(lastLoop);    
    }
}
