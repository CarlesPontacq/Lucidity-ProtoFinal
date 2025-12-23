using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioResource audio;
}

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxObject;
    
    [Header("Sound Library")]
    public Sound[] sounds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void PlaySound(string soundName, float volume)
    {
        Sound sound = Array.Find(sounds, s => s.name == soundName);
        if (sound == null) return;

        AudioSource audioSource = Instantiate(sfxObject, transform.position, Quaternion.identity);

        audioSource.resource = sound.audio;
        audioSource.volume = volume;
        audioSource.Play();

        if (sound.audio is AudioClip)
        {
            float clipLength = audioSource.clip.length;
            Destroy(audioSource.gameObject, clipLength);
        }
        else
        {
            StartCoroutine(DestroyWhenFinished(audioSource));
        }
    }

    private IEnumerator DestroyWhenFinished(AudioSource source)
    {
        while (source.isPlaying)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Destroy(source.gameObject);
    }

    // TEST
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            PlaySound("simpleAudioTest", 1f);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlaySound("clipVariantsTest", 1f);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlaySound("randomPitchVolumeTest", 1f);
        }
    }
}
