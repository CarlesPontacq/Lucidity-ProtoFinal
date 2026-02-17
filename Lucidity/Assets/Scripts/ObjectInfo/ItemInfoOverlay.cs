using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoOverlay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Image image;
    [SerializeField] Image background;

    void Start()
    {
        Hide();
    }

    public void OpenInfo(ItemData itemData)
    {
        SetInfo(itemData);
        Show();
    }

    private void SetInfo(ItemData itemData)
    {
        nameText.text = itemData.itemName;
        descriptionText.text = itemData.description;
        image.sprite = itemData.image;
    }

    private void Hide()
    {
        background.gameObject.SetActive(false);
        nameText.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
        image.gameObject.SetActive(false);
    }

    private void Show()
    {
        background.gameObject.SetActive(true);
        nameText.gameObject.SetActive(true);
        descriptionText.gameObject.SetActive(true);
        image.gameObject.SetActive(true);
    }
}
