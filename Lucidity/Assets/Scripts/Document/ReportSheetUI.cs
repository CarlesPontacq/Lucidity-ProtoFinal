using UnityEngine;
using TMPro;

public class ReportSheetUI : MonoBehaviour
{
    [SerializeField] private GameObject sheetPanel;
    [SerializeField] private TMP_InputField numberInput;

    private bool open;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            open = !open;
            sheetPanel.SetActive(open);

            Cursor.visible = open;
            Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;

            if (open)
            {
                numberInput.ActivateInputField();
                numberInput.Select();
            }
        }
    }
}
