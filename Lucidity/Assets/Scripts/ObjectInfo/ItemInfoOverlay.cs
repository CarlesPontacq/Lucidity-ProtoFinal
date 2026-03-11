using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoOverlay : MonoBehaviour
{
    [SerializeField] PlayerInputObserver playerInput;
    [SerializeField] List<GameObject> otherCanvasToHide;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Image image;
    [SerializeField] Image background;

    void Start()
    {
        Hide();
        playerInput.onCloseItemInfo += CloseInfo;
    }

    public void OpenInfo(ItemData itemData)
    {
        SetInfo(itemData);
        Show();

        Time.timeScale = 0f;
        playerInput.SwitchActionMap(PlayerInputObserver.ActionMap.ItemInfo);
    }

    public void CloseInfo()
    {
        Hide();

        Time.timeScale = 1f;
        playerInput.SwitchActionMap(PlayerInputObserver.ActionMap.Player);
    }

    private void SetInfo(ItemData itemData)
    {
        itemData.itemName.StringChanged += UpdateName;
        itemData.description.StringChanged += UpdateDescription;
        image.sprite = itemData.image;
    }

    void UpdateName(string value)
    {
        nameText.text = value;
    }

    void UpdateDescription(string value)
    {
        descriptionText.text = value;
    }

    private void Hide()
    {
        background.gameObject.SetActive(false);
        nameText.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
        image.gameObject.SetActive(false);

        foreach (GameObject otherCanvas in otherCanvasToHide)
        {
            otherCanvas.SetActive(true);
        }
    }

    private void Show()
    {
        background.gameObject.SetActive(true);
        nameText.gameObject.SetActive(true);
        descriptionText.gameObject.SetActive(true);
        image.gameObject.SetActive(true);

        foreach (GameObject otherCanvas in otherCanvasToHide)
        {
            otherCanvas.SetActive(false);
        }
    }
}
