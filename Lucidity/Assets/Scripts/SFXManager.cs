using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioResource audio;
}

public class SFXManager : MonoBehaviour
{
    enum SoundType { Global, Spatial};
    public static SFXManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxObject;
    private ObjectPool<AudioSource> sfxObjectPool;

    // Pool Settings
    private const int DefaultCapacity = 10;
    private const int MaxSize = 50;
    private const bool CollectionCheck = true;

    [Header("Sound Library")]
    public Sound[] sounds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sfxObjectPool = new ObjectPool<AudioSource>(
                CreateAudioSource,
                OnGetFromPool,
                OnReleaseToPool,
                OnDestroyPooledObject,
                CollectionCheck,
                DefaultCapacity,
                MaxSize
            );
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private AudioSource CreateAudioSource()
    {
        AudioSource newSource = Instantiate(sfxObject, transform);
        return newSource;
    }

    private void OnGetFromPool(AudioSource source)
    {
        source.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        source.transform.position = Vector3.zero;
    }

    private void OnDestroyPooledObject(AudioSource source)
    {
        Destroy(source.gameObject);
    }

    public void PlayGlobalSound(string soundName, float volume)
    {
        Sound sound = Array.Find(sounds, s => s.name == soundName);
        if (sound == null) return;

        AudioSource audioSource = sfxObjectPool.Get();
        audioSource.spatialBlend = 0;

        PlaySound(audioSource, sound, volume); 
    }

    public void PlaySpatialSound(string soundName, Vector3 position, float volume)
    {
        Sound sound = Array.Find(sounds, s => s.name == soundName);
        if (sound == null) return;

        AudioSource audioSource = sfxObjectPool.Get();

        audioSource.transform.position = position;
        audioSource.spatialBlend = 1;

        PlaySound(audioSource, sound, volume);
    }

    private void PlaySound(AudioSource audioSource, Sound sound, float volume)
    {
        audioSource.resource = sound.audio;
        audioSource.volume = volume;
        audioSource.Play();

        if (sound.audio is AudioClip)
            StartCoroutine(ReleaseAfterDelay(audioSource, audioSource.clip.length));
        else
            StartCoroutine(ReleaseWhenFinished(audioSource));
    }

    private IEnumerator ReleaseAfterDelay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        sfxObjectPool.Release(source);
    }

    private IEnumerator ReleaseWhenFinished(AudioSource source)
    {
        while (source.isPlaying)
        {
            yield return new WaitForSeconds(0.5f);
        }
        sfxObjectPool.Release(source);
    }

    // TEST
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            PlayGlobalSound("simpleAudioTest", 1f);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayGlobalSound("clipVariantsTest", 1f);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayGlobalSound("randomPitchVolumeTest", 1f);
        }
    }
}
