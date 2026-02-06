using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static GameObject PlayerRef { get; private set; }

    [SerializeField] private LoopCounter loopCounterUI;
    [SerializeField] private LoopManager loopManager;
    private int currentLoop = 0;

    [SerializeField] CameraManager cameraManager;
    [SerializeField] DocumentationMode documentationMode;
    [SerializeField] ReportSheetOverlayUI reportSheet;
    private bool cameraGrabbed = false; 
    private bool reportSheetGrabbed = false; 


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

    private void Start()
    {
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
    }

    public void ResetAndStartNextLoop()
    {
        Debug.Log("ResetAndStartNextLoop se ejecuta");
        loopManager.StartNextLoop();
    }

    public int GetCurrentLoopIndex()
    {
        return currentLoop;
    }

    public void AddLoopToCount()
    {
        currentLoop++;
        loopCounterUI.SetLoopCounterText(currentLoop);
    }

    public void ResetLoops()
    {
        currentLoop = 0;
        loopCounterUI.SetLoopCounterText(currentLoop);
    }

    public void OnExitDoorCrossed()
    {
        loopManager.StartNextLoop();
    }

    public void CameraGrabbed()
    {
        cameraGrabbed = true;
        documentationMode.isUnlocked = true;
        cameraManager.SetMode(cameraManager.cameraModes[0]);
    }

    public void ReportSheetGrabbed()
    {
        reportSheetGrabbed = true;
        reportSheet.Grab();
    }

}
