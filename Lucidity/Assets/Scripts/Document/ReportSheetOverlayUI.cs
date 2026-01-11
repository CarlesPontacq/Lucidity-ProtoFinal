using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReportSheetOverlayUI : MonoBehaviour
{
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

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Q;

    [Header("Timing")]
    [SerializeField] private float closeDelaySeconds = 2f;

    private bool open;
    private bool signedThisAttempt;
    private Coroutine closeRoutine;

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

        if (reportState != null)
            reportState.Submit(correct);

        if (exitDoor != null)
            exitDoor.UnlockExitDoor();

        if (exitBlocker != null)
            exitBlocker.UnlockPassage();

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
        closeRoutine = StartCoroutine(CloseAfterSeconds(closeDelaySeconds));
    }

    private System.Collections.IEnumerator CloseAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SetOpen(false);
        closeRoutine = null;
    }

    private void SetOpen(bool value)
    {
        open = value;
        if (sheetPanel) sheetPanel.SetActive(open);

        Cursor.visible = open;
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;

        if (open)
        {
            signedThisAttempt = false;
            if (signatureStamp) signatureStamp.gameObject.SetActive(false);

            SetFeedback("");
            numberInput?.ActivateInputField();
            numberInput?.Select();
        }
    }

    private void SetFeedback(string msg)
    {
        if (feedbackText) feedbackText.text = msg;
    }
}
