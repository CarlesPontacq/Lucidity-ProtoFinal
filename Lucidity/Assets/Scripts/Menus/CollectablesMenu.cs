using UnityEngine;

public class CollectablesMenu : MonoBehaviour
{
    [SerializeField] private InfoDisplay infoDisplay;
    [SerializeField] private ItemData[] collectables;

    [SerializeField] private GameObject previousArrow;
    [SerializeField] private GameObject nextArrow;

    private int currentIndex;

    private void Start()
    {
        ShowCurrent();
    }

    public void Next()
    {
        PlaySound();

        currentIndex++;

        if (currentIndex >= collectables.Length)
            currentIndex = 0;

        ShowCurrent();
    }

    public void Previous()
    {
        PlaySound();

        currentIndex--;

        if (currentIndex < 0)
            currentIndex = collectables.Length - 1;

        ShowCurrent();
    }

    private void ShowCurrent()
    {
        infoDisplay.SetInfo(collectables[currentIndex]);

        previousArrow.SetActive(currentIndex > 0);
        nextArrow.SetActive(currentIndex < collectables.Length - 1);
    }

    private void PlaySound()
    {
        SFXManager.Instance.PlayGlobalSound("pageTurn", 1f);
    }
}