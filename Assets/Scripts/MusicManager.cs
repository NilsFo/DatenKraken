using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class MusicManager : MonoBehaviour
{
    public static readonly float GLOBAL_GAME_VOLUME = 1.0f;

    public float globalUserVolume = 0.5f;
    public float volumeChangeRate = 0.35f;
    public float volumeDifferenceTolerance = 0.003f;

    [Header("Available tracks")] public AudioSource track1;
    public AudioSource track2;
    public AudioSource track3;
    public AudioSource track4;
    public AudioSource track5;
    public AudioSource track6;
    public AudioSource track7;
    public AudioSource track8;
    public AudioSource track9;
    public AudioSource track10;
    public AudioSource track11;
    public AudioSource track12;
    public AudioSource track13;
    public AudioSource track14;
    public AudioSource track15;
    public AudioSource track16;
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

    public MusicProfile currentProfile = MusicProfile.SILENCE;
    private MusicProfile _lastKnownProfile;

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
        _audioSources.Add(new VolumizedAudioSource(track5));
        _audioSources.Add(new VolumizedAudioSource(track6));
        _audioSources.Add(new VolumizedAudioSource(track7));
        _audioSources.Add(new VolumizedAudioSource(track8));
        _audioSources.Add(new VolumizedAudioSource(track9));
        _audioSources.Add(new VolumizedAudioSource(track10));
        _audioSources.Add(new VolumizedAudioSource(track11));
        _audioSources.Add(new VolumizedAudioSource(track12));
        _audioSources.Add(new VolumizedAudioSource(track13));
        _audioSources.Add(new VolumizedAudioSource(track14));
        _audioSources.Add(new VolumizedAudioSource(track15));
        _audioSources.Add(new VolumizedAudioSource(track16));
        SetMusicProfile(MusicProfile.SILENCE);
        _lastKnownProfile = currentProfile;

        // Keeping this
        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        print("Playing audio sources.");
        foreach (VolumizedAudioSource volumizedAudioSource in _audioSources)
        {
            print("Starting instrument: " + volumizedAudioSource.audioSource.gameObject.name);
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

        if (_lastKnownProfile != currentProfile)
        {
            OnProfileChange();
            _lastKnownProfile = currentProfile;
        }
    }


    public void SetMusicProfile(MusicProfile newProfile)
    {
        currentProfile = newProfile;
    }

    private void OnProfileChange()
    {
        print("Switching to music profile: " + currentProfile);

        // Silencing all first
        foreach (VolumizedAudioSource audioSource in _audioSources)
        {
            audioSource.volumeDesired = 0.0f;
        }

        /*
         * _audioSources[0].volumeDesired = 1.0f; // Big Sleazy
         * _audioSources[1].volumeDesired = 1.0f; // Drums
         * _audioSources[2].volumeDesired = 1.0f; // Dynamic Mini
         * _audioSources[3].volumeDesired = 1.0f; // Flöte
         * _audioSources[4].volumeDesired = 1.0f; // Good Morning
         * _audioSources[5].volumeDesired = 1.0f; // Hi Hat
         * _audioSources[6].volumeDesired = 1.0f; // Kick and Clap
         * _audioSources[7].volumeDesired = 1.0f; // Klavier
         * _audioSources[8].volumeDesired = 1.0f; // Little Bells
         * _audioSources[9].volumeDesired = 1.0f; // Lucky Manic
         * _audioSources[10].volumeDesired = 1.0f; // Percussion
         * _audioSources[11].volumeDesired = 1.0f; // Theremin
         * _audioSources[12].volumeDesired = 1.0f; // Tiefe Bläster
         * _audioSources[13].volumeDesired = 1.0f; // Tiefe Streicher
         * _audioSources[14].volumeDesired = 1.0f; // Violine
         * _audioSources[15].volumeDesired = 1.0f; // Waterpeal
         */

        print("Known instruments: " + _audioSources.Count);

        switch (currentProfile)
        {
            case MusicProfile.SILENCE:
                // Nothing to do, it's already silence
                break;
            case MusicProfile.MAIN_MENU:
                _audioSources[7].volumeDesired = 1.0f; // Klavier
                _audioSources[12].volumeDesired = 1.0f; // Tiefe Bläster
                break;
            case MusicProfile.LEVEL_1:
                _audioSources[5].volumeDesired = 1.0f; // Hi Hat
                _audioSources[7].volumeDesired = 1.0f; // Klavier
                _audioSources[10].volumeDesired = 1.0f; // Percussion
                _audioSources[2].volumeDesired = 1.0f; // Dynamic Mini
                _audioSources[14].volumeDesired = 1.0f; // Violine
                break;
            case MusicProfile.LEVEL_2:
                _audioSources[5].volumeDesired = 1.0f; // Hi Hat
                _audioSources[6].volumeDesired = 1.0f; // Kick and Clap
                _audioSources[2].volumeDesired = 1.0f; // Dynamic Mini
                _audioSources[8].volumeDesired = 1.0f; // Little Bells
                _audioSources[9].volumeDesired = 1.0f; // Lucky Manic
                _audioSources[10].volumeDesired = 1.0f; // Percussion
                _audioSources[14].volumeDesired = 1.0f; // Violine
                break;
            case MusicProfile.LEVEL_3:
                _audioSources[2].volumeDesired = 1.0f; // Dynamic Mini
                _audioSources[5].volumeDesired = 1.0f; // Hi Hat
                _audioSources[6].volumeDesired = 1.0f; // Kick and Clap
                _audioSources[7].volumeDesired = 1.0f; // Klavier
                _audioSources[9].volumeDesired = 1.0f; // Lucky Manic
                _audioSources[10].volumeDesired = 1.0f; // Percussion
                _audioSources[13].volumeDesired = 1.0f; // Tiefe Streicher
                _audioSources[14].volumeDesired = 1.0f; // Violine
                break;
            case MusicProfile.LEVEL_4:
                _audioSources[1].volumeDesired = 1.0f; // Drums
                _audioSources[2].volumeDesired = 1.0f; // Dynamic Mini
                _audioSources[4].volumeDesired = 1.0f; // Good Morning
                _audioSources[8].volumeDesired = 1.0f; // Little Bells
                _audioSources[9].volumeDesired = 1.0f; // Lucky Manic
                _audioSources[10].volumeDesired = 1.0f; // Percussion
                _audioSources[13].volumeDesired = 1.0f; // Tiefe Streicher
                _audioSources[14].volumeDesired = 1.0f; // Violine
                _audioSources[15].volumeDesired = 1.0f; // Waterpeal
                break;
            case MusicProfile.LEVEL_5:
                _audioSources[0].volumeDesired = 1.0f; // Big Sleazy
                _audioSources[1].volumeDesired = 1.0f; // Drums
                _audioSources[2].volumeDesired = 1.0f; // Dynamic Mini
                _audioSources[4].volumeDesired = 1.0f; // Good Morning
                _audioSources[5].volumeDesired = 1.0f; // Hi Hat
                _audioSources[6].volumeDesired = 1.0f; // Kick and Clap
                _audioSources[8].volumeDesired = 1.0f; // Little Bells
                _audioSources[9].volumeDesired = 1.0f; // Lucky Manic
                _audioSources[10].volumeDesired = 1.0f; // Percussion
                _audioSources[11].volumeDesired = 1.0f; // Theremin
                _audioSources[13].volumeDesired = 1.0f; // Tiefe Streicher
                _audioSources[14].volumeDesired = 1.0f; // Violine
                _audioSources[15].volumeDesired = 1.0f; // Waterpeal
                break;
            case MusicProfile.LEVEL_6:
                _audioSources[0].volumeDesired = 1.0f; // Big Sleazy
                _audioSources[1].volumeDesired = 1.0f; // Drums
                _audioSources[2].volumeDesired = 1.0f; // Dynamic Mini
                _audioSources[3].volumeDesired = 1.0f; // Flöte
                _audioSources[4].volumeDesired = 1.0f; // Good Morning
                _audioSources[5].volumeDesired = 1.0f; // Hi Hat
                _audioSources[6].volumeDesired = 1.0f; // Kick and Clap
                _audioSources[7].volumeDesired = 1.0f; // Klavier
                _audioSources[8].volumeDesired = 1.0f; // Little Bells
                _audioSources[9].volumeDesired = 1.0f; // Lucky Manic
                _audioSources[10].volumeDesired = 1.0f; // Percussion
                _audioSources[11].volumeDesired = 1.0f; // Theremin
                _audioSources[12].volumeDesired = 1.0f; // Tiefe Bläster
                _audioSources[13].volumeDesired = 1.0f; // Tiefe Streicher
                _audioSources[14].volumeDesired = 1.0f; // Violine
                _audioSources[15].volumeDesired = 1.0f; // Waterpeal
                break;
            default:
                Debug.LogError("WARNING! UNKNOWN MUSIC STATE!");
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