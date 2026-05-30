using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LanguageSelector : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text languageText;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [Header("Languages")]
    [SerializeField]
    private string[] languages =
    {
        "Català",
        "Español",
        "English"
    };

    private int currentLanguageIndex = 0;

    private void Start()
    {
        leftButton.onClick.AddListener(PreviousLanguage);
        rightButton.onClick.AddListener(NextLanguage);

        LoadLanguage();
        UpdateLanguageText();
    }

    private void PreviousLanguage()
    {
        currentLanguageIndex--;

        if (currentLanguageIndex < 0)
        {
            currentLanguageIndex = languages.Length - 1;
        }

        UpdateLanguageText();
        SaveLanguage();
    }

    private void NextLanguage()
    {
        currentLanguageIndex++;

        if (currentLanguageIndex >= languages.Length)
        {
            currentLanguageIndex = 0;
        }

        UpdateLanguageText();
        SaveLanguage();
    }

    private void UpdateLanguageText()
    {
        languageText.text = languages[currentLanguageIndex];
    }

    private void SaveLanguage()
    {
        PlayerPrefs.SetInt("LanguageIndex", currentLanguageIndex);
        PlayerPrefs.Save();

        Debug.Log("Idioma seleccionado: " + languages[currentLanguageIndex]);
    }

    private void LoadLanguage()
    {
        currentLanguageIndex = PlayerPrefs.GetInt("LanguageIndex", 0);
    }
}