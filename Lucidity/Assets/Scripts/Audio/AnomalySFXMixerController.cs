using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class CameraAudioMixerController : MonoBehaviour
{
    private const string ANOMALY_VOLUME = "AnomalyVolume";
    private const string REALITY_VOLUME = "RealityVolume";

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private CameraManager cameraManager;

    [Header("Camera Transition Timing")]
    [SerializeField] private float enterCameraTime = 0f;
    [SerializeField] private float exitCameraTime = 0.5f;

    [Header("Volume (dB)")]
    [SerializeField] private float mutedDb = -80f;
    [SerializeField] private float normalDb = 0f;

    void OnEnable()
    {
        if (cameraManager == null)
            cameraManager = FindAnyObjectByType<CameraManager>();

        cameraManager.OnCameraLookedThrough += OnCameraEnabled;
        cameraManager.OnCameraStoppedLookingThrough += OnCameraDisabled;
    }

    void OnDisable()
    {
        if (cameraManager == null)
            return;

        cameraManager.OnCameraLookedThrough -= OnCameraEnabled;
        cameraManager.OnCameraStoppedLookingThrough -= OnCameraDisabled;
    }

    void Start()
    {
        SetImmediate(ANOMALY_VOLUME, mutedDb);
        SetImmediate(REALITY_VOLUME, normalDb);
    }

    private void OnCameraEnabled()
    {
        TransitionAudio(normalDb, mutedDb, enterCameraTime);
    }

    private void OnCameraDisabled()
    {
        TransitionAudio(mutedDb, normalDb, exitCameraTime);
    }

    private void TransitionAudio(float anomalyTarget, float realityTarget, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeMixer(ANOMALY_VOLUME, anomalyTarget, duration));
        StartCoroutine(FadeMixer(REALITY_VOLUME, realityTarget, duration));
    }

    private IEnumerator FadeMixer(string parameter, float targetDb, float duration)
    {
        mixer.GetFloat(parameter, out float currentDb);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, time / duration);
            mixer.SetFloat(parameter, Mathf.Lerp(currentDb, targetDb, t));
            yield return null;
        }

        mixer.SetFloat(parameter, targetDb);
    }

    private void SetImmediate(string parameter, float value)
    {
        mixer.SetFloat(parameter, value);
    }
}