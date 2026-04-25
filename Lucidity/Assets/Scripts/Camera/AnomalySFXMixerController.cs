using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AnomalySFXMixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private CameraManager cameraManager;

    [Header("Timing")]
    [SerializeField] private float timeToHearAnomalies = 0f;
    [SerializeField] private float timeToStopHearingAnomalies = 0.5f;

    [Header("Volume (dB)")]
    [SerializeField] private float mutedDb = -80f;
    [SerializeField] private float normalDb = 0f;

    private Coroutine currentFade;

    void Start()
    {
        if (cameraManager == null)
            cameraManager = FindAnyObjectByType<CameraManager>();

        cameraManager.OnCameraLookedThrough += EnableAnomalyAudio;
        cameraManager.OnCameraStoppedLookingThrough += DisableAnomalyAudio;

        SetImmediate(mutedDb);
    }

    private void EnableAnomalyAudio()
    {
        StartFade(normalDb, timeToHearAnomalies);
    }

    private void DisableAnomalyAudio()
    {
        StartFade(mutedDb, timeToStopHearingAnomalies);
    }

    private void StartFade(float targetDb, float duration)
    {
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeMixer(targetDb, duration));
    }

    private IEnumerator FadeMixer(float targetDb, float duration)
    {
        mixer.GetFloat("AnomalyVolume", out float currentDb);

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            t = Mathf.SmoothStep(0f, 1f, t);

            float newDb = Mathf.Lerp(currentDb, targetDb, t);
            mixer.SetFloat("AnomalyVolume", newDb);

            yield return null;
        }

        mixer.SetFloat("AnomalyVolume", targetDb);
    }

    private void SetImmediate(float value)
    {
        mixer.SetFloat("AnomalyVolume", value);
    }
}