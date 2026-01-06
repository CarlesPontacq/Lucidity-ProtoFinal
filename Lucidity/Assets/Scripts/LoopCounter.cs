using TMPro;
using UnityEngine;

public class LoopCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textComponent;

    public void SetLoopCounterText(int count)
    {
        textComponent.text = count.ToString();
    }
}
