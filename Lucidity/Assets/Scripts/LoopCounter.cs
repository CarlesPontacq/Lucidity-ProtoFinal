using TMPro;
using UnityEngine;

public class LoopCounter : MonoBehaviour
{
    [SerializeField] TextMeshPro textComponent;

    public void SetLoopCounterText(int count)
    {
        textComponent.text = count.ToString();
    }
}
