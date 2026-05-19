using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class InfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image displayImage;

    private LocalizedString currentTitle;
    private LocalizedString currentDescription;

    public void SetInfo(ItemData data)
    {
        Unsubscribe();

        currentTitle = data.itemName;
        currentDescription = data.description;

        currentTitle.StringChanged += UpdateTitle;
        currentDescription.StringChanged += UpdateDescription;

        if (data.image != null)
        {
            displayImage.gameObject.SetActive(true);
            displayImage.sprite = data.image;
        }
        else
        {
            displayImage.sprite = null;
            displayImage.gameObject.SetActive(false);
        }

        UpdateTitle(currentTitle.GetLocalizedString());
        UpdateDescription(currentDescription.GetLocalizedString());
    }

    private void UpdateTitle(string value)
    {
        titleText.text = value;
    }

    private void UpdateDescription(string value)
    {
        descriptionText.text = value;
    }

    private void Unsubscribe()
    {
        if (currentTitle != null)
            currentTitle.StringChanged -= UpdateTitle;

        if (currentDescription != null)
            currentDescription.StringChanged -= UpdateDescription;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}

