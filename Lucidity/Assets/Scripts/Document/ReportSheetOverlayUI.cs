using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ReportSheetOverlayUI : MonoBehaviour
{
    public static bool IsOpen { get; private set; }

    [Header("UI Root")]
    [SerializeField] private GameObject sheetPanel;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Number Options")]
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private GameObject[] circleMarkers;

    [Header("Signature")]
    [SerializeField] private Button signatureButton;

    [Header("Signature Blink")]
    [SerializeField] private GameObject signatureBlinkObject;
    [SerializeField] private Image signatureBlinkImage;
    [SerializeField] private float blinkFadeDuration = 1.2f;
    [SerializeField] private float blinkMinAlpha = 0.2f;
    [SerializeField] private float blinkMaxAlpha = 1f;
    [SerializeField] private AnimationCurve blinkCurve = null;

    [Header("Signature Write")]
    [SerializeField] private GameObject signatureWriteObject;
    [SerializeField] private Animator signatureWriteAnimator;
    [SerializeField] private string signatureWriteStateName = "SignatureWrite";
    [SerializeField] private float signatureWriteDuration = 1.2f;

    [Header("Stamp")]
    [SerializeField] private GameObject stampObject;
    [SerializeField] private Image signatureStamp;
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
    [SerializeField] private ExitLamp exitLamp;

    [Header("Input")]
    [SerializeField] private PlayerInputObserver playerInput;

    [Header("Timing")]
    [SerializeField] private float closeDelaySeconds = 2f;

    [Header("Pause")]
    [SerializeField] private bool pauseGameWhenOpen = true;

    [Header("Disable mouse/world interactions while open")]
    [SerializeField] private MonoBehaviour[] disableWhileOpen;

    [Header("SFX")]
    [SerializeField] private string selectNumberAnomaliesSFX = "ReportCircle";
    [SerializeField] private string signatureSFX = "ReportFirma";
    private float sfxVolume = 1.0f;

    public bool open;

    private bool signedThisAttempt;
    private bool selectionLocked;
    private int selectedNumber = -1;

    private Coroutine closeRoutine;
    private float previousTimeScale = 1f;

    public event Action OnOpened;
    public event Action OnClosed;
    public event Action OnNumberSelected;
    public event Action OnSigned;

    private bool canOpen = false;

    private float blinkTime = 0f;
    private bool blinkGoingUp = true;

    private void Awake()
    {
        if (blinkCurve == null || blinkCurve.length == 0)
            blinkCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        if (playerInput != null)
            playerInput.onToggleSheet += ToggleSheet;

        BindButtons();
        SetOpen(false);
        ResetDocumentState();
    }

    private void Update()
    {
        if (open)
        {
            UpdateSignatureBlink();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (open && Input.GetKeyDown(KeyCode.Tab))
            SetOpen(false);
    }

    private void LateUpdate()
    {
        if (open)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
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
        Debug.Log("SELECT NUMBER CLICKED: " + number);

        if (!open) return;
        if (selectionLocked) return;
        if (number < 0 || number >= 4) return;

        OnNumberSelected?.Invoke();

        selectedNumber = number;

        HideAllCircles();

        if (circleMarkers != null && number < circleMarkers.Length && circleMarkers[number] != null)
        {
            circleMarkers[number].SetActive(true);

            Animator circleAnimator = circleMarkers[number].GetComponent<Animator>();
            if (circleAnimator != null)
                circleAnimator.SetTrigger("Select");

            SFXManager.Instance.PlayGlobalSound(selectNumberAnomaliesSFX, sfxVolume);
        }

        if (signatureButton != null)
            signatureButton.interactable = true;

        if (signatureBlinkObject != null)
        {
            signatureBlinkObject.SetActive(true);
            ResetBlinkState();
        }

        SetFeedback("");
    }

    public void OnSignatureClicked()
    {
        Debug.Log("CLICK FIRMA DETECTADO");

        if (!open) return;
        if (signedThisAttempt) return;

        if (selectedNumber < 0)
        {
            SetFeedback("Selecciona un n�mero primero.");
            return;
        }

        SFXManager.Instance.PlayGlobalSound(signatureSFX, sfxVolume);

        signedThisAttempt = true;
        selectionLocked = true;

        OnSigned?.Invoke();

        SetOptionButtonsInteractable(false);

        if (signatureButton != null)
            signatureButton.interactable = false;

        if (closeRoutine != null)
            StopCoroutine(closeRoutine);

        closeRoutine = StartCoroutine(SignAndSubmitRoutine());
    }

    private IEnumerator SignAndSubmitRoutine()
    {
        Debug.Log("[UI] Starting signature animation");

        if (signatureBlinkObject != null)
            signatureBlinkObject.SetActive(false);

        if (signatureWriteObject != null)
        {
            signatureWriteObject.SetActive(true);
            signatureWriteObject.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogWarning("[UI] signatureWriteObject es NULL");
        }

        if (signatureWriteAnimator != null)
            signatureWriteAnimator.Play(signatureWriteStateName, 0, 0f);
        else
            Debug.LogWarning("[UI] signatureWriteAnimator es NULL");

        yield return new WaitForSecondsRealtime(signatureWriteDuration);

        Debug.Log("[UI] Intentando mostrar stamp");

        if (stampObject != null)
        {
            stampObject.SetActive(true);
            stampObject.transform.SetAsLastSibling();

            Image stampImage = stampObject.GetComponent<Image>();
            if (stampImage != null)
            {
                Color c = stampImage.color;
                c.a = 1f;
                stampImage.color = c;
            }

            Debug.Log("[UI] Stamp activado");
        }
        else
        {
            Debug.LogWarning("[UI] stampObject es NULL");
        }

        if (stampAnimator != null)
        {
            stampAnimator.Play(0, 0, 0f);
            Debug.Log("[UI] Stamp animator reproducido");
        }
        else
        {
            Debug.LogWarning("[UI] stampAnimator es NULL");
        }

        if (stampObject != null)
        {
            stampObject.SetActive(true);
            stampObject.transform.SetAsLastSibling();
        }

        if (stampAnimator != null)
            stampAnimator.Play(stampStateName, 0, 0f);

        yield return new WaitForSecondsRealtime(stampDuration);

        int expected = anomalyManager.GetExpectedAnomalies();
        bool correct = selectedNumber == expected;

        UnlockNextLoop(correct);

        SetFeedback(correct
            ? "Correcto. Ya puedes pasar por la puerta."
            : "Incorrecto. Ya puedes pasar por la puerta.");

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
            exitLamp = FindAnyObjectByType<ExitLamp>();

        if (exitLamp != null)
            exitLamp.TurnOff();
        else
            Debug.LogWarning("[UI] exitLamp NO encontrada/asignada. No puedo poner verde.");

        GameManager.Instance.HasFinishedLastLoop();
    }

    private void SetOpen(bool value)
    {
        if (PauseMenuManager.IsOpen) return;

        IsOpen = value;
        open = value;

        if (sheetPanel != null)
            sheetPanel.SetActive(open);
        if (open)
            OnOpened?.Invoke();
        else
            OnClosed?.Invoke();

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
    }

    public void ResetDocumentState()
    {
        signedThisAttempt = false;
        selectionLocked = false;
        selectedNumber = -1;

        HideAllCircles();

        if (signatureBlinkObject != null)
            signatureBlinkObject.SetActive(false);

        if (signatureWriteObject != null)
            signatureWriteObject.SetActive(false);

        if (stampObject != null)
            stampObject.SetActive(false);

        if (signatureStamp != null)
            signatureStamp.gameObject.SetActive(false);

        if (signatureButton != null)
            signatureButton.interactable = false;

        ResetBlinkState();
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

    private void ResetBlinkState()
    {
        blinkTime = 0f;
        blinkGoingUp = true;

        if (signatureBlinkImage != null)
        {
            Color c = signatureBlinkImage.color;
            c.a = blinkMinAlpha;
            signatureBlinkImage.color = c;
        }
    }

    private void UpdateSignatureBlink()
    {
        if (signatureBlinkObject == null || !signatureBlinkObject.activeSelf) return;
        if (signatureBlinkImage == null) return;

        blinkTime += Time.unscaledDeltaTime;

        float t = Mathf.Clamp01(blinkTime / blinkFadeDuration);
        float curveValue = blinkCurve.Evaluate(t);

        float alpha = blinkGoingUp
            ? Mathf.Lerp(blinkMinAlpha, blinkMaxAlpha, curveValue)
            : Mathf.Lerp(blinkMaxAlpha, blinkMinAlpha, curveValue);

        Color c = signatureBlinkImage.color;
        c.a = alpha;
        signatureBlinkImage.color = c;

        if (blinkTime >= blinkFadeDuration)
        {
            blinkTime = 0f;
            blinkGoingUp = !blinkGoingUp;
        }
    }

    private void OnDisable()
    {
        IsOpen = false;

        if (pauseGameWhenOpen && open)
            Time.timeScale = previousTimeScale;

        SetWorldInteractionsEnabled(true);
    }
}