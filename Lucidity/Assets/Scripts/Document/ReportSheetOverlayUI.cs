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
    [SerializeField] private LoopManager loopManager; 

    private bool open;
    private bool signedThisAttempt;

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
        if (Input.GetKeyDown(KeyCode.Q))
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

        int expected = anomalyManager.ActiveSpawnedCount;

        if (guess == expected)
        {
            Debug.Log($"Firmado y correcto. Puesto={guess}, Esperado={expected}");
            SetFeedback("Correcto.");

            // avanza al siguiente loop 
            if (loopManager != null)
                loopManager.StartNextLoop();

            // espera 2s 
            StartCoroutine(CloseAfterSeconds(0.5f));
        }
        else
        {
            Debug.Log($"Firmado pero incorrecto. Puesto={guess}, Esperado={expected}");
            SetFeedback("Incorrecto. Sigues en el mismo loop.");

            // Permite reintentar
            signedThisAttempt = false;
            if (signatureStamp) signatureStamp.gameObject.SetActive(false);
        }
    }

    private System.Collections.IEnumerator CloseAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SetOpen(false);
    }



    private void SetOpen(bool value)
    {
        open = value;
        sheetPanel.SetActive(open);

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
