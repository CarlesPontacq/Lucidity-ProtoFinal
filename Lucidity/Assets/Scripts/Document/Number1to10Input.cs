using TMPro;
using UnityEngine;

public class Number1to10Input : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;

    public int Value { get; private set; } = -1;
    public bool IsValid => Value >= 1 && Value <= 10;

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
        // limpia cosas raras (por si pegan texto)
        if (string.IsNullOrEmpty(text))
        {
            Value = -1;
            return;
        }

        if (!int.TryParse(text, out var n))
        {
            Value = -1;
            return;
        }

        Value = n;
    }

    private void OnEndEdit(string text)
    {
        // clamp final (cuando el jugador termina)
        if (!int.TryParse(text, out var n))
        {
            input.text = "";
            Value = -1;
            return;
        }

        if (n < 1) n = 1;
        if (n > 10) n = 10;

        Value = n;
        input.text = n.ToString();
    }
}
