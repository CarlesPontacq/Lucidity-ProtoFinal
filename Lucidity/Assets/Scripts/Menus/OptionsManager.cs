using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    [SerializeField] private TMP_Dropdown languageDropdown;

    [Header("References")]
    [SerializeField] private CameraRotation cameraRotation;

    private const string SOUND_MUTE = "sound_mute";
    private const string SOUND_VOL = "sound_vol";
    private const string MUSIC_MUTE = "music_mute";
    private const string MUSIC_VOL = "music_vol";
    private const string MOUSE_SENS = "mouse_sens";
    private const string LANGUAGE = "language";

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

        if(languageDropdown != null)
            languageDropdown.SetValueWithoutNotify(languageIndex);
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetInt(SOUND_MUTE, isSoundMute ? 1 : 0);
        PlayerPrefs.SetFloat(SOUND_VOL, soundVolume);

        PlayerPrefs.SetInt(MUSIC_MUTE, isMusicMute ? 1 : 0);
        PlayerPrefs.SetFloat(MUSIC_VOL, musicVolume);

        PlayerPrefs.SetFloat(MOUSE_SENS, mouseSensitivity);

        PlayerPrefs.SetInt(LANGUAGE, languageIndex);

        PlayerPrefs.Save();
    }

    public void ApplyOptions()
    {
        ApplySound();
        ApplySensitivity();
    }

    IEnumerator StartLocalization()
    {
        yield return LocalizationSettings.InitializationOperation;

        var options = new List<TMP_Dropdown.OptionData>();
        int selected = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
                selected = i;
            options.Add(new TMP_Dropdown.OptionData(locale.name));
        }
        languageDropdown.options = options;

        languageDropdown.value = selected;
        languageDropdown.onValueChanged.AddListener(LocaleSelected);
    }

    static void LocaleSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }

    private void ApplySound()
    {
        if (SFXManager.Instance != null)
            SFXManager.Instance.SetVolume(isSoundMute ? 0f : soundVolume);

        // MusicManager.Instance.SetVolume(isMusicMute ? 0f : musicVolume);
    }

    private void ApplySensitivity()
    {
        if (cameraRotation != null)
            cameraRotation.SetSensitivity(mouseSensitivity);
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
        SaveOptions();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        SaveOptions();
    }

    public void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
        ApplySensitivity();
        SaveOptions();
    }
}
