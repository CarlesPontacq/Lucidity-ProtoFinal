using UnityEngine;
using TMPro;

public class ReportSheetOverlayUI : MonoBehaviour
{
    [SerializeField] private GameObject sheetPanel;   
    [SerializeField] private TMP_InputField numberInput;

    [Header("Opcional: desactiva movimiento FPS mientras está abierto")]
    [SerializeField] private MonoBehaviour fpsMoveScript;

    private bool isOpen;

    private void Awake()
    {
        SetOpen(false);

        if (numberInput)
        {
            numberInput.contentType = TMP_InputField.ContentType.IntegerNumber;
            numberInput.characterLimit = 2;
            numberInput.onEndEdit.AddListener(OnEndEdit);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            SetOpen(!isOpen);

        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
            SetOpen(false);

        if (Input.GetKeyDown(KeyCode.E)) Debug.Log("E!");

    }

    private void OnEndEdit(string text)
    {
        if (!numberInput) return;

        if (!int.TryParse(text, out int n))
        {
            numberInput.text = "";
            return;
        }

        n = Mathf.Clamp(n, 1, 10);
        numberInput.text = n.ToString();
    }

    private void SetOpen(bool open)
    {
        isOpen = open;

        if (sheetPanel) sheetPanel.SetActive(open);

        if (fpsMoveScript) fpsMoveScript.enabled = !open;

        Cursor.visible = open;
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;

        if (open && numberInput)
        {
            numberInput.ActivateInputField();
            numberInput.Select();
        }
    }
}
