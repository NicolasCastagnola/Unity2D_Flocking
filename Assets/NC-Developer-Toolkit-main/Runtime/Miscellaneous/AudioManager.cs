using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    private AudioSource firstMusicSource;
    private AudioSource secondMusicSource;
    private AudioSource effectsSource;

    private bool firstMusicSourceIsPlaying = true;
    private bool isMuted;

    [HideInInspector] public AudioClip currentAudioClipPlaying;

    public static AudioManager Instance
    {
        get
        {
            if (_instance != null) return _instance;
            
            _instance = FindObjectOfType<AudioManager>();

            if (_instance == null)
            {
                _instance = new GameObject("Audio Manager", typeof(AudioManager)).GetComponent<AudioManager>();
            }

            return _instance;
        }
        
        set => _instance = value;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        firstMusicSource = gameObject.AddComponent<AudioSource>();
        secondMusicSource = gameObject.AddComponent<AudioSource>();
        effectsSource = gameObject.AddComponent<AudioSource>();

        firstMusicSource.loop = true;
        secondMusicSource.loop = true;
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (isMuted) return;

        var activeMusic = (firstMusicSourceIsPlaying) ? firstMusicSource : secondMusicSource;

        currentAudioClipPlaying = musicClip;
        activeMusic.clip = musicClip;
        activeMusic.volume = 1f;
        activeMusic.Play();
    }
    
    public void PlayMusicWithCrossFade(AudioClip musicClip, float transitionTime = 1f)
    {
        if (isMuted) return;
        
        var activeMusic = (firstMusicSourceIsPlaying) ? firstMusicSource : secondMusicSource;
        var newMusic = (firstMusicSourceIsPlaying) ? secondMusicSource : firstMusicSource;

        firstMusicSourceIsPlaying = !firstMusicSourceIsPlaying;
        
        currentAudioClipPlaying = musicClip;
        activeMusic.clip = musicClip;
        activeMusic.Play();

        StartCoroutine(UpdateMusicWithCrossFade(activeMusic, newMusic, transitionTime));
    }

    public void PlayMusicWithFade(AudioClip audioClip, float transitionTime = 1f)
    {
        if (isMuted) return;
        
        var activeMusic = (firstMusicSourceIsPlaying) ? firstMusicSource : secondMusicSource;

        currentAudioClipPlaying = audioClip;
        
        StartCoroutine(UpdateMusicWithFade(activeMusic, audioClip, transitionTime));

    }

    private IEnumerator UpdateMusicWithFade(AudioSource activeSource, AudioClip newClip, float transitionTime)
    {
        if (!activeSource.isPlaying) activeSource.Play();

        var t = 0.0f;

        for (t = 0; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (1 - (t / transitionTime));
            yield return null;
        }
        
        currentAudioClipPlaying = newClip;
        
        activeSource.Stop();
        activeSource.clip = newClip;
        activeSource.Play();
        
        for (t = 0; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (t / transitionTime);
            yield return null;
        }

    }
    private IEnumerator UpdateMusicWithCrossFade(AudioSource activeSource, AudioSource newSource, float transitionTime)
    {
        if (!activeSource.isPlaying) activeSource.Play();

        var t = 0.0f;

        for (t = 0.0f; t <= transitionTime; t += transitionTime)
        {
            activeSource.volume = (1 - (t / transitionTime));
            newSource.volume = (t / transitionTime);
            yield return null;
        }
        
        activeSource.Stop();
    }


    public void PlaySoundFX(AudioClip clip)
    {
        effectsSource.PlayOneShot(clip);
    }
    public void SetMasterVolume(float volume)
    {
        firstMusicSource.volume = volume;
        secondMusicSource.volume = volume;
        effectsSource.volume = volume;

        isMuted = !isMuted;
    }
    
}
