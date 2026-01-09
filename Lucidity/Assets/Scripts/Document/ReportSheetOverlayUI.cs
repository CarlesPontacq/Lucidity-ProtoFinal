using UnityEngine;
using TMPro;

public class ReportSheetOverlayUI : MonoBehaviour
{
    [SerializeField] private GameObject sheetPanel;
    [SerializeField] private TMP_InputField numberInput;
    [SerializeField] private TMP_Text feedbackText;

    [SerializeField] private AnomalyManager anomalyManager;
    [SerializeField] private LoopManager loopManager;

    private bool open;

    private void Awake()
    {
        SetOpen(false);
        numberInput.contentType = TMP_InputField.ContentType.IntegerNumber;

        numberInput.onSubmit.AddListener(_ => Submit());
        numberInput.onEndEdit.AddListener(_ =>
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                Submit();
        });

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            SetOpen(!open);

        if (open && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            ValidateAndLog();
    }

    private void ValidateAndLog()
    {
        Debug.Log($"[UI] anomalyManager={anomalyManager.name} id={anomalyManager.GetInstanceID()} ActiveSpawnedCount={anomalyManager.ActiveSpawnedCount} EntryCount={anomalyManager.EntryCount}");

        if (!int.TryParse(numberInput.text, out int guess) || guess < 0)
        {
            Debug.Log("Número inválido en el documento.");
            return;
        }

        int expected = anomalyManager.ActiveSpawnedCount; 


        if (guess == expected)
            Debug.Log($"Correcto: {guess} (Loop total = {expected})");
        else
            Debug.Log($"Incorrecto: pusiste {guess}, pero el total es {expected}");
    }



    public void Submit()
    {
        if (!int.TryParse(numberInput.text, out int guess) || guess < 0)
        {
            SetFeedback("Introduce un número válido.");
            return;
        }

        int expected = anomalyManager.ActiveSpawnedCount;

        if (guess == expected)
        {
            SetFeedback("Correcto.");
            SetOpen(false);
            loopManager.StartNextLoop();
        }
        else
        {
            SetFeedback("Incorrecto. Sigues en el mismo loop.");
        }
    }


    private void SetOpen(bool value)
    {
        open = value;
        sheetPanel.SetActive(open);

        Cursor.visible = open;
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;

        if (open)
        {
            SetFeedback("");
            numberInput.ActivateInputField();
            numberInput.Select();
        }
    }

    private void SetFeedback(string msg)
    {
        if (feedbackText) feedbackText.text = msg;
    }
}
