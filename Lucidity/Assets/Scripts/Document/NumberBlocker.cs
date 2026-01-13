using TMPro;
using UnityEngine;

public class NumberBlocker : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;

    public int Value { get; private set; } = -1;
    public bool IsValid => Value >= 0 && Value <= 99;

    private void Reset()
    {
        input = GetComponent<TMP_InputField>();
    }

    private void Awake()
    {
        if (!input) input = GetComponent<TMP_InputField>();

        input.contentType = TMP_InputField.ContentType.IntegerNumber;
        input.characterLimit = 2;

        input.onValueChanged.AddListener(OnChanged);
        input.onEndEdit.AddListener(OnEndEdit);
    }

    private void OnChanged(string text)
    {
        // Si está vacío, no hay valor
        if (string.IsNullOrEmpty(text))
        {
            Value = -1;
            return;
        }

        // Si no es número, invalida
        if (!int.TryParse(text, out var n))
        {
            Value = -1;
            return;
        }

        Value = n;
    }

    private void OnEndEdit(string text)
    {
        // Clamp final al terminar
        if (!int.TryParse(text, out var n))
        {
            input.text = "";
            Value = -1;
            return;
        }

        n = Mathf.Clamp(n, 0, 99);

        Value = n;
        input.text = n.ToString();
    }
}
