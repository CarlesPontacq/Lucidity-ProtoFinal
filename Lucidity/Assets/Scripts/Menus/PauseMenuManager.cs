using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public static bool IsOpen { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Button returnToMainMenuButton;

    [Header("Input")]
    [SerializeField] private PlayerInputObserver playerInput;
    [SerializeField] private GameObject overlayUI;

    [Header("Pause")]
    [SerializeField] private bool pauseGameWhenOpen = true;

    private bool openPausePanel;
    private bool isOptionsPanelOpen;
    private float previousTimeScale = 1f;

    private void Awake()
    {
        playerInput.onPause += TogglePause;

    }

    private void Start()
    {
        returnToMainMenuButton.onClick.AddListener(SceneController.Instance.LoadMainMenuScene);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        SetOpen(!openPausePanel);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        isOptionsPanelOpen = true;
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        isOptionsPanelOpen = false;
    }

    public void SetOpen(bool value)
    {
        if (ReportSheetOverlayUI.IsOpen) return;
        if (GameManager.Instance.isDying) return;
        if(isOptionsPanelOpen) return;

        openPausePanel = value;
        IsOpen = openPausePanel;

        if (pausePanel)
            pausePanel.SetActive(openPausePanel);

        Cursor.visible = openPausePanel;
        Cursor.lockState = openPausePanel ? CursorLockMode.None : CursorLockMode.Locked;

        if (pauseGameWhenOpen)
        {
            if (openPausePanel)
            {
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = previousTimeScale;
            }
        }

        if (playerInput != null)
        {
            if (openPausePanel)
                playerInput.SwitchActionMap(PlayerInputObserver.ActionMap.UI);
            else
                playerInput.SwitchActionMap(PlayerInputObserver.ActionMap.Player);
        }
    }

    private void OnDisable()
    {
        IsOpen = false;

        if (pauseGameWhenOpen && openPausePanel)
            Time.timeScale = previousTimeScale;
    }
}
