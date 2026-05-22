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

    [Header("Language")]
    public int languageIndex = 0;

    [SerializeField] private TMP_Text languageText;
    [SerializeField] private Button languageLeftButton;
    [SerializeField] private Button languageRightButton;

    [SerializeField]
    private string[] languageDisplayNames =
    {
    "Català",
    "Español",
    "English"
};

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

        languageIndex = Mathf.Clamp(
            languageIndex,
            0,
            LocalizationSettings.AvailableLocales.Locales.Count - 1
        );

        LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.Locales[languageIndex];

        UpdateLanguageUI();

        languageLeftButton.onClick.AddListener(PreviousLanguage);
        languageRightButton.onClick.AddListener(NextLanguage);
    }

    public void PreviousLanguage()
    {
        languageIndex--;

        if (languageIndex < 0)
        {
            languageIndex =
                LocalizationSettings.AvailableLocales.Locales.Count - 1;
        }

        ApplyLanguage();
    }

    public void NextLanguage()
    {
        languageIndex++;

        if (languageIndex >= LocalizationSettings.AvailableLocales.Locales.Count)
        {
            languageIndex = 0;
        }

        ApplyLanguage();
    }

    private void ApplyLanguage()
    {
        LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.Locales[languageIndex];

        UpdateLanguageUI();
        SaveOptions();
    }

    private void UpdateLanguageUI()
    {
        if (languageText == null)
            return;

        if (languageDisplayNames == null || languageDisplayNames.Length == 0)
            return;

        int displayIndex = Mathf.Clamp(languageIndex, 0, languageDisplayNames.Length - 1);

        languageText.text = languageDisplayNames[displayIndex];
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
}