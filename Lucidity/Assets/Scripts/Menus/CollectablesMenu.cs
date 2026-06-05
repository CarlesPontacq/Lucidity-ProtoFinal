using System.Collections.Generic;
using UnityEngine;

public class CollectablesMenu : MonoBehaviour
{
    [SerializeField] private InfoDisplay infoDisplay;
    [SerializeField] private ItemData lockedCollectable;

    [SerializeField] private ItemData[] collectablesAct1;
    [SerializeField] private ItemData[] collectablesAct2;
    [SerializeField] private ItemData[] collectablesAct3;

    [SerializeField] private GameObject previousArrow;
    [SerializeField] private GameObject nextArrow;

    private List<ItemData> pages = new List<ItemData>();
    private int currentIndex;

    private const string COMPLETED_ACTS = "completed_acts";

    private void Start()
    {
        BuildPages();
        ShowCurrent();
    }

    private void BuildPages()
    {
        pages.Clear();

        int completedActs = PlayerPrefs.GetInt(COMPLETED_ACTS, 0);

        if (completedActs <= 0)
        {
            pages.Add(lockedCollectable);
            return;
        }

        AddRange(collectablesAct1);

        if (completedActs >= 2)
        {
            AddRange(collectablesAct2);
        }

        if (completedActs >= 3)
        {
            AddRange(collectablesAct3);
        }
        else
        {
            pages.Add(lockedCollectable);
        }

        if (pages.Count == 0)
            pages.Add(lockedCollectable);
    }

    private void AddRange(ItemData[] arr)
    {
        if (arr == null) return;
        for (int i = 0; i < arr.Length; i++)
            pages.Add(arr[i]);
    }

    public void Next()
    {
        PlaySound();

        currentIndex++;
        if (currentIndex >= pages.Count)
            currentIndex = 0;

        ShowCurrent();
    }

    public void Previous()
    {
        PlaySound();

        currentIndex--;
        if (currentIndex < 0)
            currentIndex = pages.Count - 1;

        ShowCurrent();
    }

    private void ShowCurrent()
    {
        infoDisplay.SetInfo(pages[currentIndex]);

        previousArrow.SetActive(currentIndex > 0);
        nextArrow.SetActive(currentIndex < pages.Count - 1);
    }

    private void PlaySound()
    {
        SFXManager.Instance.PlayGlobalSound("pageTurn", 1f);
    }
}