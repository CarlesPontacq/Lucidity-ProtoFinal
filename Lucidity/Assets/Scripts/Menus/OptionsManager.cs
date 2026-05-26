using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [Header("Sound")]
    public bool isSoundMute = false;
    [Range(0f, 1f)] public float soundVolume = 1f;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Toggle soundMuteToggle;

    [Header("Music")]
    public bool isMusicMute = false;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle musicMuteToggle;

    [Header("Sensitivity")]
    public float mouseSensitivity = 0.75f;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Text sensitivityValueText;

    [Header("Language")]
    public int languageIndex = 0;

    [SerializeField] private TMP_Text languageText;
    [SerializeField] private Button languageLeftButton;
    [SerializeField] private Button languageRightButton;

    [System.Serializable]
    public class LanguageOption
    {
        public string displayName;
        public string localeCode;
    }

    [SerializeField] private LanguageOption[] languages;

    [Header("Sprint")]
    public int sprintIndex = 0;
    public bool isToggleSprint = false;

    [Header("References")]
    [SerializeField] private CameraRotation cameraRotation;
    [SerializeField] private AudioSource ostMixer;

    private const string SOUND_MUTE = "sound_mute";
    private const string SOUND_VOL = "sound_vol";
    private const string MUSIC_MUTE = "music_mute";
    private const string MUSIC_VOL = "music_vol";
    private const string MOUSE_SENS = "mouse_sens";
    private const string LANGUAGE = "language";
    private const string SPRINT_MODE = "sprint_mode";

    void Start()
    {
        StartCoroutine(StartLocalization());
        LoadOptions();
        ApplyOptions();
        RefreshUI();
    }

    public void LoadOptions()
    {
        isSoundMute = PlayerPrefs.GetInt(SOUND_MUTE, 0) == 1;
        soundVolume = PlayerPrefs.GetFloat(SOUND_VOL, 1f);

        isMusicMute = PlayerPrefs.GetInt(MUSIC_MUTE, 0) == 1;
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOL, musicVolume);

        mouseSensitivity = PlayerPrefs.GetFloat(MOUSE_SENS, mouseSensitivity);

        languageIndex = PlayerPrefs.GetInt(LANGUAGE, 0);

        sprintIndex = PlayerPrefs.GetInt(SPRINT_MODE, 0);
        isToggleSprint = sprintIndex == 1;
    }

    private void RefreshUI()
    {
        if (soundSlider != null)
            soundSlider.SetValueWithoutNotify(soundVolume);

        if (soundMuteToggle != null)
            soundMuteToggle.SetIsOnWithoutNotify(isSoundMute);

        if (musicSlider != null)
            musicSlider.SetValueWithoutNotify(musicVolume);

        if (musicMuteToggle != null)
            musicMuteToggle.SetIsOnWithoutNotify(isMusicMute);

        if (sensitivitySlider != null)
            sensitivitySlider.SetValueWithoutNotify(mouseSensitivity);

        UpdateSensitivityText();
        UpdateLanguageUI();
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetInt(SOUND_MUTE, isSoundMute ? 1 : 0);
        PlayerPrefs.SetFloat(SOUND_VOL, soundVolume);

        PlayerPrefs.SetInt(MUSIC_MUTE, isMusicMute ? 1 : 0);
        PlayerPrefs.SetFloat(MUSIC_VOL, musicVolume);

        PlayerPrefs.SetFloat(MOUSE_SENS, mouseSensitivity);

        PlayerPrefs.SetInt(LANGUAGE, languageIndex);

        PlayerPrefs.SetInt(SPRINT_MODE, sprintIndex);

        PlayerPrefs.Save();
    }

    public void ApplyOptions()
    {
        ApplySound();
        ApplySensitivity();
        ApplySprintMode();
    }

    IEnumerator StartLocalization()
    {
        yield return LocalizationSettings.InitializationOperation;

        languageIndex = Mathf.Clamp(languageIndex, 0, languages.Length - 1);
        Debug.Log("Index: " + languageIndex + " - Size: " + languages.Length);

        ApplyLanguage();

        languageLeftButton.onClick.AddListener(PreviousLanguage);
        languageRightButton.onClick.AddListener(NextLanguage);
    }

    public void PreviousLanguage()
    {
        languageIndex--;

        if (languageIndex < 0)
            languageIndex = languages.Length - 1;

        ApplyLanguage();
    }

    public void NextLanguage()
    {
        languageIndex++;

        if (languageIndex >= languages.Length)
            languageIndex = 0;

        ApplyLanguage();
    }

    private void ApplyLanguage()
    {
        string selectedCode = languages[languageIndex].localeCode;

        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if (locale.Identifier.Code == selectedCode)
            {
                LocalizationSettings.SelectedLocale = locale;
                break;
            }
        }

        UpdateLanguageUI();
        SaveOptions();
    }

    private void UpdateLanguageUI()
    {
        if (languageText == null)
            return;

        if (languages == null || languages.Length == 0)
            return;

        languageText.text = languages[languageIndex].displayName;
    }

    private void ApplySound()
    {
        if (SFXManager.Instance != null)
            SFXManager.Instance.SetVolume(isSoundMute ? 0f : soundVolume);

        if (ostMixer != null)
            ostMixer.volume = musicVolume;
    }

    private void ApplySensitivity()
    {
        if (cameraRotation != null)
            cameraRotation.SetSensitivity(mouseSensitivity);
    }

    private void ApplySprintMode()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SetToggleSprint(isToggleSprint);
    }

    public void SetSoundMute(bool value)
    {
        isSoundMute = value;
        ApplySound();
        SaveOptions();
    }

    public void SetSoundVolume(float value)
    {
        soundVolume = value;
        ApplySound();
        SaveOptions();
    }

    public void SetMusicMute(bool value)
    {
        isMusicMute = value;
        ApplySound();
        SaveOptions();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        ApplySound();
        SaveOptions();
    }

    public void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
        UpdateSensitivityText();
        ApplySensitivity();
        SaveOptions();
    }

    public void OnSprintModeChanged(int index)
    {
        sprintIndex = index;
        isToggleSprint = index == 1;

        ApplySprintMode();
        SaveOptions();
    }

    private void UpdateSensitivityText()
    {
        if (sensitivityValueText == null)
            return;

        int percentage = Mathf.RoundToInt(mouseSensitivity * 100f);
        sensitivityValueText.text = percentage.ToString();
    }
}