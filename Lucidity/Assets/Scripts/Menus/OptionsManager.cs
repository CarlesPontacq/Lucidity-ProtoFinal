using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance { get; private set; }

    [Header("Sound")]
    public bool isSoundMute = false;
    [Range(0f, 1f)] public float soundVolume = 1f;

    [Header("Music")]
    public bool isMusicMute = false;
    [Range(0f, 1f)] public float musicVolume = 1f;

    [Header("Sensitivity")]
    public float mouseSensitivity = 0.75f;

    [Header("References")]
    [SerializeField] private CameraRotation cameraRotation;

    [SerializeField] private Slider soundSlider;
    [SerializeField] private Toggle soundMuteToggle;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle musicMuteToggle;

    [SerializeField] private Slider sensitivitySlider;

    private const string SOUND_MUTE = "sound_mute";
    private const string SOUND_VOL = "sound_vol";
    private const string MUSIC_MUTE = "music_mute";
    private const string MUSIC_VOL = "music_vol";
    private const string MOUSE_SENS = "mouse_sens";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
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
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetInt(SOUND_MUTE, isSoundMute ? 1 : 0);
        PlayerPrefs.SetFloat(SOUND_VOL, soundVolume);

        PlayerPrefs.SetInt(MUSIC_MUTE, isMusicMute ? 1 : 0);
        PlayerPrefs.SetFloat(MUSIC_VOL, musicVolume);

        PlayerPrefs.SetFloat(MOUSE_SENS, mouseSensitivity);

        PlayerPrefs.Save();
    }

    public void ApplyOptions()
    {
        ApplySound();
        ApplySensitivity();
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
