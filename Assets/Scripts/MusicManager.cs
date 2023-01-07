using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class MusicManager : MonoBehaviour
{
    public static readonly float GLOBAL_GAME_VOLUME = 0.25f;

    public float globalUserVolume = 0.5f;
    public float volumeChangeRate = 0.35f;
    public float volumeDifferenceTolerance = 0.003f;

    [Header("Available tracks")] public AudioSource track1;
    public AudioSource track2;
    public AudioSource track3;
    public AudioSource track4;
    public AudioSource track5;
    private List<VolumizedAudioSource> _audioSources;

    public enum MusicProfile
    {
        SILENCE,
        MAIN_MENU,
        LEVEL_1,
        LEVEL_2,
        LEVEL_3,
        LEVEL_4,
        LEVEL_5,
        LEVEL_6
    }

    private void Awake()
    {
        MusicManager[] multipleManagers = FindObjectsOfType<MusicManager>();
        if (multipleManagers.Length > 1)
        {
            Debug.LogWarning("THERE IS A MUSIC IMPOSTER AMOGUS!");
            Destroy(gameObject);
            return;
        }

        // Setting up lists
        _audioSources = new List<VolumizedAudioSource>();
        _audioSources.Add(new VolumizedAudioSource(track1));
        _audioSources.Add(new VolumizedAudioSource(track2));
        _audioSources.Add(new VolumizedAudioSource(track3));
        _audioSources.Add(new VolumizedAudioSource(track4));
        SetMusicProfile(MusicProfile.SILENCE);

        // Keeping this
        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        foreach (VolumizedAudioSource volumizedAudioSource in _audioSources)
        {
            volumizedAudioSource.audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (VolumizedAudioSource audioSource in _audioSources)
        {
            float volumeCurrent = audioSource.volumeCurrent;
            float volumeDesired = audioSource.volumeDesired;

            if (volumeCurrent < volumeDesired)
            {
                volumeCurrent = volumeCurrent + volumeChangeRate * Time.deltaTime;
            }

            if (volumeCurrent > volumeDesired)
            {
                volumeCurrent = volumeCurrent - volumeChangeRate * Time.deltaTime;
            }

            volumeCurrent = MathF.Max(volumeCurrent, 0);
            volumeCurrent = Mathf.Min(volumeCurrent, 1);

            if (Math.Abs(volumeCurrent - volumeDesired) < volumeDifferenceTolerance)
            {
                volumeCurrent = volumeDesired;
            }

            // Storing changed volume
            audioSource.volumeCurrent = volumeCurrent;

            // Applying global volume and setting the audio source volume
            audioSource.audioSource.volume = volumeCurrent * globalUserVolume * GLOBAL_GAME_VOLUME;
        }
    }

    public void SetMusicProfile(MusicProfile newProfile)
    {
        print("Switching to music profile: " + newProfile);

        // Silencing all first
        foreach (VolumizedAudioSource audioSource in _audioSources)
        {
            audioSource.volumeDesired = 0.0f;
        }

        switch (newProfile)
        {
            case MusicProfile.SILENCE:
                // Nothing to do, it's already silence
                break;
            case MusicProfile.MAIN_MENU:
                _audioSources[0].volumeDesired = 1.0f;
                break;
            case MusicProfile.LEVEL_1:
                _audioSources[0].volumeDesired = 1.0f;
                _audioSources[1].volumeDesired = 1.0f;
                _audioSources[2].volumeDesired = 1.0f;
                _audioSources[3].volumeDesired = 1.0f;
                _audioSources[4].volumeDesired = 1.0f;
                _audioSources[5].volumeDesired = 1.0f;
                break;
            case MusicProfile.LEVEL_2:
                break;
            case MusicProfile.LEVEL_3:
                break;
            case MusicProfile.LEVEL_4:
                break;
            case MusicProfile.LEVEL_5:
                break;
            case MusicProfile.LEVEL_6:
                break;
        }
    }

    public void SkipFadeIn()
    {
        foreach (var audioSource in _audioSources)
        {
            audioSource.volumeCurrent = audioSource.volumeDesired;
        }
    }

    private class VolumizedAudioSource
    {
        public float volumeCurrent;
        public float volumeDesired;
        public AudioSource audioSource;

        public VolumizedAudioSource(AudioSource audioSource)
        {
            this.volumeDesired = 0;
            this.volumeCurrent = 0;
            this.audioSource = audioSource;
        }
    }
}