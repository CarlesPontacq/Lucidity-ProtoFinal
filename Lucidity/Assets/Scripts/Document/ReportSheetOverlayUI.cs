using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    [SerializeField] private KeyCode toggleKey = KeyCode.Q;

    [Header("Timing")]
    [SerializeField] private float closeDelaySeconds = 2f;

    [Header("Pause")]
    [SerializeField] private bool pauseGameWhenOpen = true;

    [Header("Disable mouse/world interactions while open")]
    [SerializeField] private MonoBehaviour[] disableWhileOpen;

    private bool open;
    private bool signedThisAttempt;
    private Coroutine closeRoutine;
    private float previousTimeScale = 1f;

    private void Awake()
    {
        SetOpen(false);

        if (numberInput)
            numberInput.contentType = TMP_InputField.ContentType.IntegerNumber;

        if (signatureStamp)
            signatureStamp.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            SetOpen(!open);

        if (open && Input.GetKeyDown(KeyCode.Escape))
            SetOpen(false);
    }

    public void OnSignatureClicked()
    {
        if (!open) return;
        if (signedThisAttempt) return;

        signedThisAttempt = true;
        if (signatureStamp) signatureStamp.gameObject.SetActive(true);

        // Validar número
        if (!int.TryParse(numberInput.text, out int guess) || guess < 0)
        {
            SetFeedback("Introduce un número válido.");
            signedThisAttempt = false;
            if (signatureStamp) signatureStamp.gameObject.SetActive(false);
            return;
        }

        int expected = (anomalyManager != null) ? anomalyManager.ActiveSpawnedCount : 0;
        bool correct = (guess == expected);

        // Guardar resultado
        if (reportState != null)
            reportState.Submit(correct);

        // Desbloquear puerta/paso (siempre al firmar)
        if (exitDoor != null)
            exitDoor.UnlockExitDoor();

        if (exitBlocker != null)
            exitBlocker.UnlockPassage();

        // Cambiar lámpara a verde (siempre al firmar)
        if (exitLamp != null)
        {
            exitLamp.SetCanPass(true);
        }
        else
        {
            Debug.LogWarning("ReportSheetOverlayUI: exitLamp NO asignada (no puedo poner la luz en verde).");
        }

        if (correct)
        {
            Debug.Log($"Firmado y correcto. Puesto={guess}, Esperado={expected}");
            SetFeedback("Correcto.");
        }
        else
        {
            Debug.Log($"Firmado pero incorrecto. Puesto={guess}, Esperado={expected}");
            SetFeedback("Incorrecto. Ya puedes pasar por la puerta.");
        }

        if (closeRoutine != null) StopCoroutine(closeRoutine);
        closeRoutine = StartCoroutine(CloseAfterSecondsRealtime(closeDelaySeconds));
    }

    private System.Collections.IEnumerator CloseAfterSecondsRealtime(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        SetOpen(false);
        closeRoutine = null;
    }

    private void SetOpen(bool value)
    {
        IsOpen = value;

        open = value;
        if (sheetPanel) sheetPanel.SetActive(open);

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
