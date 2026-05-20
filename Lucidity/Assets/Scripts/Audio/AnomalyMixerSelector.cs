using UnityEngine;
using UnityEngine.Audio;

public class AnomalyAudioSelector : MonoBehaviour
{
    private const string NORMAL_OBJECT_LAYER_NAME = "Normal Object";

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixerGroup generalGroup;
    [SerializeField] private AudioMixerGroup realityGroup;

    private bool anomalyActive;

    void Awake()
    {
        LoopManager.OnLoopStarted += OnLoopStarted;
    }

    void OnDestroy()
    {
        LoopManager.OnLoopStarted -= OnLoopStarted;
    }

    private void OnLoopStarted(int loopIndex)
    {
        ApplyRouting();
    }

    private void ApplyRouting()
    {
        bool hasAnomaly = gameObject.layer == LayerMask.NameToLayer(NORMAL_OBJECT_LAYER_NAME);
        audioSource.outputAudioMixerGroup = hasAnomaly ? realityGroup : generalGroup;
    }
}