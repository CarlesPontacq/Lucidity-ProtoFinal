using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ReportSheetOverlayUI : MonoBehaviour
{
    public static bool IsOpen { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject sheetPanel;
    [SerializeField] private TMP_InputField numberInput;
    [SerializeField] private Image signatureStamp;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Game")]
    [SerializeField] private AnomalyManager anomalyManager;
    [SerializeField] private DoorInteraction exitDoor;
    [SerializeField] private ReportResultState reportState;

    [Header("Exit Blocker (optional)")]
    [SerializeField] private ExitDoorBlocker exitBlocker;

    [Header("Exit Lamp (optional)")]
    [SerializeField] private ExitLightEmissionMapSwitcher exitLamp;

    [Header("Input")]
    [SerializeField] PlayerInputObserver playerInput;

    [Header("Timing")]
    [SerializeField] private float closeDelaySeconds = 2f;

    [Header("Pause")]
    [SerializeField] private bool pauseGameWhenOpen = true;

    [Header("Disable mouse/world interactions while open")]
    [SerializeField] private MonoBehaviour[] disableWhileOpen;

    public bool open;
    private bool signedThisAttempt;
    private Coroutine closeRoutine;
    private float previousTimeScale = 1f;

    public event Action OnReportSheetOpenedFirstTime;
    private bool hasOpenedOnce = false;

    private bool canOpen = false;

    private void Awake()
    {
        playerInput.onToggleSheet += ToggleSheet;

        SetOpen(false);

        if (numberInput)
            numberInput.contentType = TMP_InputField.ContentType.IntegerNumber;

        if (signatureStamp)
            signatureStamp.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (open && Input.GetKeyDown(KeyCode.Tab))
            SetOpen(false);
    }

    public void Grab()
    {
        canOpen = true;
    }

    void ToggleSheet()
    {
        if (PauseMenuManager.IsOpen) return;
        if (!canOpen || reportState.HasSubmittedReport) return;

        SFXManager.Instance.PlayGlobalSound("paper", 0.3f);
        SetOpen(!open);
    }

    public void OnSignatureClicked()
    {
        Debug.Log("[UI] OnSignatureClicked()");

        if (!open) return;
        if (signedThisAttempt) return;

        signedThisAttempt = true;
        if (signatureStamp) signatureStamp.gameObject.SetActive(true);

        if (!int.TryParse(numberInput.text, out int guess) || guess < 0)
        {
            SetFeedback("Introduce un número válido.");
            signedThisAttempt = false;
            if (signatureStamp) signatureStamp.gameObject.SetActive(false);
            return;
        }

        if (anomalyManager == null)
        {
            Debug.LogWarning("[UI] anomalyManager es null.");
            SetFeedback("Error: AnomalyManager no asignado.");
            signedThisAttempt = false;
            if (signatureStamp) signatureStamp.gameObject.SetActive(false);
            return;
        }

        int expected = anomalyManager.GetExpectedAnomalies();
        bool correct = (guess == expected);

        UnlockNextLoop(correct);

        if (correct)
        {
            Debug.Log($"Firmado y correcto. Puesto={guess}, Esperado={expected}");
            SetFeedback("Correcto. Ya puedes pasar por la puerta.");
        }
        else
        {
            Debug.Log($"Firmado pero incorrecto. Puesto={guess}, Esperado={expected}");
            SetFeedback("Incorrecto. Ya puedes pasar por la puerta.");
        }

        if (closeRoutine != null) StopCoroutine(closeRoutine);
        closeRoutine = StartCoroutine(CloseAfterSecondsRealtime(closeDelaySeconds));
    }

    public void UnlockNextLoop(bool correctSubmission)
    {
        if (reportState != null)
            reportState.Submit(correctSubmission);

        if (exitDoor != null)
            exitDoor.Unlock();

        if (exitBlocker != null)
            exitBlocker.UnlockPassage();

        if (exitLamp == null)
            exitLamp = FindAnyObjectByType<ExitLightEmissionMapSwitcher>();

        if (exitLamp != null)
            exitLamp.SetCanPass(true);
        else
            Debug.LogWarning("[UI] exitLamp NO encontrada/asignada. No puedo poner verde.");
    }

    private System.Collections.IEnumerator CloseAfterSecondsRealtime(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        SetOpen(false);
        closeRoutine = null;
    }

    private void SetOpen(bool value)
    {
        if (PauseMenuManager.IsOpen) return;

        IsOpen = value;

        open = value;
        if (sheetPanel) sheetPanel.SetActive(open);


        if (!hasOpenedOnce && open)
        {
            hasOpenedOnce = true;
            OnReportSheetOpenedFirstTime?.Invoke();
        }

        Cursor.visible = open;
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;

        if (pauseGameWhenOpen)
        {
            if (open)
            {
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = previousTimeScale;
            }
        }

        SetWorldInteractionsEnabled(!open);

        if (open)
        {
            signedThisAttempt = false;
            if (signatureStamp) signatureStamp.gameObject.SetActive(false);

            SetFeedback("");
            numberInput?.ActivateInputField();
            numberInput?.Select();
        }
    }

    private void SetWorldInteractionsEnabled(bool enabled)
    {
        if (disableWhileOpen == null) return;

        for (int i = 0; i < disableWhileOpen.Length; i++)
        {
            if (disableWhileOpen[i] != null)
                disableWhileOpen[i].enabled = enabled;
        }

        if (enabled)
            playerInput.SwitchActionMap(PlayerInputObserver.ActionMap.Player);
        else
            playerInput.SwitchActionMap(PlayerInputObserver.ActionMap.ReportSheet);
    }

    private void SetFeedback(string msg)
    {
        if (feedbackText) feedbackText.text = msg;
    }

    private void OnDisable()
    {
        IsOpen = false;

        if (pauseGameWhenOpen && open)
            Time.timeScale = previousTimeScale;

        SetWorldInteractionsEnabled(true);
    }
}
