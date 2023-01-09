using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameState : MonoBehaviour
{
    public enum PlayerState
    {
        PAUSED, // TODO USE?
        PLAYING,
        WIN
    }

    public PlayerState playerState;
    private PlayerState lastKnownState;

    public Datenkrake player;
    public NPCCursorAI cursor;

    [Header("Objective")] public int objectiveTarget;
    public int objectiveCurrent;
    public TMP_Text objectiveProgressText;

    [Header("Main Menu?")] public float backToMainMenuTimeWindow = 2.0f;
    private float backToMainMenuTimer = 0;
    public GameObject backToMenuTF;
    public bool IsBackToMenuTimeWindow => backToMainMenuTimer > 0;

    [Header("Win?")] public string nextLevelName = "";
    public float nextLevelTimer = 2.0f;
    private float _nextLevelProgress = 0.0f;

    [Header("Tutorial")] public bool isTutorialLevel = false;

    [Header("Music")] public GameObject musicManagerPrefab;
    public MusicManager musicManager;
    public MusicManager.MusicProfile levelMusicProfile = MusicManager.MusicProfile.SILENCE;

    [Header("Sounds")] public AudioSource stingerGood;
    public AudioSource stingerBad;
    public AudioSource stingerLevelWin;

    // Camera Shake
    [Header("Camera Shake")] public GameObject CMCameraFocus;
    public float cameraShakeMagnitude = 0f;
    public float cameraShakeDuration = 0f;
    private float _cameraShakeDurationTimer = 0f;

    private void Awake()
    {
        playerState = PlayerState.PLAYING;
        lastKnownState = playerState;
        player = FindObjectOfType<Datenkrake>();
        cursor = FindObjectOfType<NPCCursorAI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        backToMainMenuTimer = 0;
        SetupMusic();
    }

    // Update is called once per frame
    void Update()
    {
        if (lastKnownState != playerState)
        {
            OnGameStateChange();
            lastKnownState = playerState;
        }

        // Camera Shake
        if (cameraShakeMagnitude <= 0 || _cameraShakeDurationTimer >= cameraShakeDuration)
        {
            ResetCameraShake();
        }

        if (cameraShakeDuration > 0)
        {
            _cameraShakeDurationTimer += Time.deltaTime;
        }

        if (playerState == PlayerState.WIN)
        {
            _nextLevelProgress += Time.deltaTime;
            if (_nextLevelProgress >= nextLevelTimer)
            {
                NextLevel();
            }
        }

        // Updating volume
        stingerBad.volume = musicManager.globalUserVolume * MusicManager.GLOBAL_GAME_SOUNDS_VOLUME;
        stingerGood.volume = musicManager.globalUserVolume * MusicManager.GLOBAL_GAME_SOUNDS_VOLUME;
        stingerLevelWin.volume = musicManager.globalUserVolume * MusicManager.GLOBAL_GAME_SOUNDS_VOLUME;

        // Back To MainMenu
        backToMainMenuTimer = backToMainMenuTimer - Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
            // TODO input system?
        {
            if (playerState == PlayerState.WIN)
            {
                BackToMainMenu();
            }

            if (IsBackToMenuTimeWindow)
            {
                BackToMainMenu();
            }
            else
            {
                backToMainMenuTimer = backToMainMenuTimeWindow;
            }
        }

        backToMenuTF.SetActive(IsBackToMenuTimeWindow);

        objectiveProgressText.text = "Data harvested: " + objectiveCurrent + "/" + objectiveTarget;
        if (playerState == PlayerState.PLAYING)
        {
            if (CheckWinCondition())
            {
                playerState = PlayerState.WIN;
                musicManager.SetMusicProfile(MusicManager.MusicProfile.SILENCE);
            }
        }
    }

    public bool CheckWinCondition()
    {
        if (objectiveTarget > 0)
        {
            if (objectiveCurrent >= objectiveTarget)
            {
                return true;
            }
        }

        return false;
    }

    private void NextLevel()
    {
        SceneManager.LoadScene(nextLevelName);
    }

    public void IncreaseObjectiveTarget()
    {
        objectiveTarget++;
    }

    public void IncreaseObjectiveProgress()
    {
        objectiveCurrent++;
    }

    private void OnGameStateChange()
    {
        switch (playerState)
        {
            case PlayerState.WIN:
                Debug.Log("A WINNER IS YOU!");
                stingerGood.Stop();
                stingerLevelWin.Play();
                break;
            case PlayerState.PLAYING:
                Debug.Log("You are now playing.");
                break;
            case PlayerState.PAUSED:
                Debug.LogWarning("The state is set to 'paused' but nothing happens!");
                break;
            default:
                Debug.LogWarning("WARNING! UNKNOWN STATE!");
                break;
        }
    }

    [ContextMenu("Win")]
    public void Win()
    {
        playerState = PlayerState.WIN;
        ResetCameraShake();
    }


    private void LateUpdate()
    {
        Vector3 pos = CMCameraFocus.transform.localPosition;
        pos.x = 0;
        pos.y = 0;
        pos.z = 0;

        if (cameraShakeDuration > 0 && _cameraShakeDurationTimer < cameraShakeDuration)
        {
            // Shaking it!
            pos.x = Random.Range(cameraShakeMagnitude * -1, cameraShakeMagnitude);
            pos.y = Random.Range(cameraShakeMagnitude * -1, cameraShakeMagnitude);
            // Debug.Log("Camera shake!", CMCameraFocus);
        }

        CMCameraFocus.transform.localPosition = pos;
    }

    public void ShakeCamera(float magnitude, float duration)
    {
        //print("Request to shake the camera by " + magnitude + " for " + duration);
        if (magnitude >= cameraShakeMagnitude)
        {
            //print("Request accepted.");
            cameraShakeMagnitude = magnitude;
            cameraShakeDuration = duration;
        }
        else
        {
            //print("Magnitude too low. Denied.");
        }
    }

    public void ResetCameraShake()
    {
        cameraShakeMagnitude = 0;
        cameraShakeDuration = 0;
        _cameraShakeDurationTimer = 0;
    }

    public void OnCloseableAdAppeared(AdvertismentCloseButton advertismentCloseButton)
    {
        if (cursor != null)
        {
            cursor.ResetLookingForButtonTimer();
            cursor.RequestInterrupt();
        }
    }

    public bool ControlsEnabled()
    {
        return playerState == PlayerState.PLAYING;
    }

    [ContextMenu("Back to Main Menu")]
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SoftResetLevel()
    {
        stingerBad.Play();

        SoftResetable[] resetables = FindObjectsOfType<SoftResetable>();
        foreach (SoftResetable resetable in resetables)
        {
            resetable.CallReset();
        }
    }

    public void SetupMusic()
    {
        MusicManager existingMusicManager = FindObjectOfType<MusicManager>();
        if (existingMusicManager == null)
        {
            GameObject newMusicManager = Instantiate(musicManagerPrefab, transform);
        }

        musicManager = FindObjectOfType<MusicManager>();
        musicManager.SetMusicProfile(levelMusicProfile);
    }
}