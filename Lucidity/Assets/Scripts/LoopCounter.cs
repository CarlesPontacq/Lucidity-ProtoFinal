using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class LoopCounter : MonoBehaviour
{
    [SerializeField] TextMeshPro textComponent;
    [SerializeField] GameObject safeTextObject;

    public void SetLoopCounterText(int count)
    {
        textComponent.text = count.ToString();

        if (count == 0)
            safeTextObject.SetActive(true);
        else
            safeTextObject.SetActive(false);

    }
}
