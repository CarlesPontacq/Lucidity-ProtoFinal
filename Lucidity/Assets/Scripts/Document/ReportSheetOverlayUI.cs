using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReportSheetOverlayUI : MonoBehaviour
{
    public static bool IsOpen { get; private set; }

    [Header("UI Root")]
    [SerializeField] private GameObject sheetPanel;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Number Options")]
    [SerializeField] private Button[] optionButtons;          // 4 botones
    [SerializeField] private GameObject[] circleMarkers;      // 4 círculos/animaciones

    [Header("Signature")]
    [SerializeField] private Button signatureButton;

    [Header("Signature Visuals")]
    [SerializeField] private GameObject signatureBlinkObject; // firma parpadeando
    [SerializeField] private GameObject signatureWriteObject; // firma escribiéndose
    [SerializeField] private Animator signatureWriteAnimator;
    [SerializeField] private string signatureWriteStateName = "SignatureWrite";
    [SerializeField] private float signatureWriteDuration = 1.2f;

    [Header("Stamp Visuals")]
    [SerializeField] private GameObject stampObject;
    [SerializeField] private Animator stampAnimator;
    [SerializeField] private string stampStateName = "StampPop";
    [SerializeField] private float stampDuration = 0.8f;

    [Header("Game")]
    [SerializeField] private AnomalyManager anomalyManager;
    [SerializeField] private DoorInteraction exitDoor;
    [SerializeField] private ReportResultState reportState;

    [Header("Exit Blocker (optional)")]
    [SerializeField] private ExitDoorBlocker exitBlocker;

    [Header("Exit Lamp (optional)")]
    [SerializeField] private ExitLightEmissionMapSwitcher exitLamp;

    [Header("Input")]
    [SerializeField] private PlayerInputObserver playerInput;

    [Header("Timing")]
    [SerializeField] private float closeDelaySeconds = 2f;

    [Header("Pause")]
    [SerializeField] private bool pauseGameWhenOpen = true;

    [Header("Disable mouse/world interactions while open")]
    [SerializeField] private MonoBehaviour[] disableWhileOpen;

    public bool open;
    private bool signedThisAttempt;
    private bool selectionLocked;
    private int selectedNumber = -1;

    private Coroutine closeRoutine;
    private float previousTimeScale = 1f;
    private bool canOpen = false;

    private void Awake()
    {
        if (playerInput != null)
            playerInput.onToggleSheet += ToggleSheet;

        BindButtons();
        SetOpen(false);
        ResetDocumentState();
    }

    private void BindButtons()
    {
        if (optionButtons != null)
        {
            for (int i = 0; i < optionButtons.Length; i++)
            {
                int index = i;
                if (optionButtons[i] != null)
                    optionButtons[i].onClick.AddListener(() => SelectNumber(index));
            }
        }

        if (signatureButton != null)
            signatureButton.onClick.AddListener(OnSignatureClicked);
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

    private void ToggleSheet()
    {
        if (PauseMenuManager.IsOpen) return;
        if (!canOpen) return;
        if (reportState != null && reportState.HasSubmittedReport) return;

        SFXManager.Instance.PlayGlobalSound("paper", 0.3f);
        SetOpen(!open);
    }

    public void SelectNumber(int number)
    {
        if (!open) return;
        if (selectionLocked) return;
        if (number < 0 || number >= 4) return;

        selectedNumber = number;

        HideAllCircles();

        if (circleMarkers != null && number < circleMarkers.Length && circleMarkers[number] != null)
        {
            circleMarkers[number].SetActive(true);

            Animator circleAnimator = circleMarkers[number].GetComponent<Animator>();
            if (circleAnimator != null)
                circleAnimator.Play(0, 0, 0f);
        }

        SetFeedback("");
    }

    public void OnSignatureClicked()
    {
        Debug.Log("[UI] OnSignatureClicked()");

        if (!open) return;
        if (signedThisAttempt) return;

        if (selectedNumber < 0)
        {
            SetFeedback("Selecciona un número primero.");
            return;
        }

        if (anomalyManager == null)
        {
            Debug.LogWarning("[UI] anomalyManager es null.");
            SetFeedback("Error: AnomalyManager no asignado.");
            return;
        }

        signedThisAttempt = true;
        selectionLocked = true;
        SetOptionButtonsInteractable(false);

        if (closeRoutine != null)
            StopCoroutine(closeRoutine);

        closeRoutine = StartCoroutine(SignAndSubmitRoutine());
    }

    private IEnumerator SignAndSubmitRoutine()
    {
        // 1) quitar firma parpadeando
        if (signatureBlinkObject != null)
            signatureBlinkObject.SetActive(false);

        // 2) reproducir firma escribiéndose
        if (signatureWriteObject != null)
            signatureWriteObject.SetActive(true);

        if (signatureWriteAnimator != null)
            signatureWriteAnimator.Play(signatureWriteStateName, 0, 0f);

        yield return new WaitForSecondsRealtime(signatureWriteDuration);

        // 3) reproducir sello
        if (stampObject != null)
            stampObject.SetActive(true);

        if (stampAnimator != null)
            stampAnimator.Play(stampStateName, 0, 0f);

        yield return new WaitForSecondsRealtime(stampDuration);

        // 4) validar
        int expected = anomalyManager.GetExpectedAnomalies();
        bool correct = (selectedNumber == expected);

        UnlockNextLoop(correct);

        if (correct)
        {
            Debug.Log($"Firmado y correcto. Puesto={selectedNumber}, Esperado={expected}");
            SetFeedback("Correcto. Ya puedes pasar por la puerta.");
        }
        else
        {
            Debug.Log($"Firmado pero incorrecto. Puesto={selectedNumber}, Esperado={expected}");
            SetFeedback("Incorrecto. Ya puedes pasar por la puerta.");
        }

        yield return new WaitForSecondsRealtime(closeDelaySeconds);

        SetOpen(false);
        closeRoutine = null;
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

    private void SetOpen(bool value)
    {
        if (PauseMenuManager.IsOpen) return;

        IsOpen = value;
        open = value;

        if (sheetPanel != null)
            sheetPanel.SetActive(open);

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
            ResetDocumentState();
    }

    private void ResetDocumentState()
    {
        signedThisAttempt = false;
        selectionLocked = false;
        selectedNumber = -1;

        HideAllCircles();

        if (signatureBlinkObject != null)
            signatureBlinkObject.SetActive(true);

        if (signatureWriteObject != null)
            signatureWriteObject.SetActive(false);

        if (stampObject != null)
            stampObject.SetActive(false);

        SetOptionButtonsInteractable(true);
        SetFeedback("");
    }

    private void HideAllCircles()
    {
        if (circleMarkers == null) return;

        for (int i = 0; i < circleMarkers.Length; i++)
        {
            if (circleMarkers[i] != null)
                circleMarkers[i].SetActive(false);
        }
    }

    private void SetOptionButtonsInteractable(bool value)
    {
        if (optionButtons == null) return;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (optionButtons[i] != null)
                optionButtons[i].interactable = value;
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

        if (playerInput != null)
        {
            if (enabled)
                playerInput.SwitchActionMap(PlayerInputObserver.ActionMap.Player);
            else
                playerInput.SwitchActionMap(PlayerInputObserver.ActionMap.ReportSheet);
        }
    }

    private void SetFeedback(string msg)
    {
        if (feedbackText != null)
            feedbackText.text = msg;
    }

    private void OnDisable()
    {
        IsOpen = false;

        if (pauseGameWhenOpen && open)
            Time.timeScale = previousTimeScale;

        SetWorldInteractionsEnabled(true);
    }
}