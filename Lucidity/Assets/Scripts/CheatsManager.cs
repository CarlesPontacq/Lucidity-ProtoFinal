using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatsManager : MonoBehaviour
{
    public static CheatsManager Instance { get; private set; }

    ReportSheetOverlayUI reportSheetScript;
    PlayerLooper playerLooper;

    [SerializeField] KeyCode nextSceneKey = KeyCode.N;
    [SerializeField] KeyCode prevSceneKey = KeyCode.P;
    [SerializeField] KeyCode openDoorKey = KeyCode.O;
    [SerializeField] KeyCode immortalityKey = KeyCode.I;
    [SerializeField] KeyCode nextLoopKey = KeyCode.L;
    [SerializeField] KeyCode restartLoopsKey = KeyCode.R;
    [SerializeField] KeyCode unlockDoorsKey = KeyCode.Alpha4;
    [SerializeField] KeyCode goToLastLoopKey = KeyCode.Alpha8;

    public bool currentlyImmortal = false;

    [SerializeField] private int unlockZoneLoop = 3;
    [SerializeField] private int lasthLoop = 7;

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

        if (playerLooper == null)
            playerLooper = FindAnyObjectByType<PlayerLooper>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(nextSceneKey))
            SceneController.Instance.LoadNextScene();

        if (Input.GetKeyDown(prevSceneKey))
            SceneController.Instance.LoadPrevScene();

        if(reportSheetScript != null)
        {
            if (Input.GetKeyDown(nextLoopKey) && playerLooper != null)
            {
                reportSheetScript.UnlockNextLoop(true);
                playerLooper.PlayerLoopCheat();
            }

            if (Input.GetKeyDown(restartLoopsKey))
            {
                reportSheetScript.UnlockNextLoop(false);
                playerLooper.PlayerLoopCheat();
                GameManager.Instance.ResetLoops();
            }

            if (Input.GetKeyDown(openDoorKey))
                reportSheetScript.UnlockNextLoop(true);
        }

        if (Input.GetKeyDown(immortalityKey))
            currentlyImmortal = !currentlyImmortal;

        if (Input.GetKeyDown(unlockDoorsKey))
            GameManager.Instance.SetCurrentLoopIndex(unlockZoneLoop);

        if (Input.GetKeyDown(goToLastLoopKey))
            GameManager.Instance.SetCurrentLoopIndex(lasthLoop);    
    }
}
