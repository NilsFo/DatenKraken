using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameState : MonoBehaviour
{
    public enum PlayerState
    {
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

    // Camera Shake
    public GameObject CMCameraFocus;
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


        objectiveProgressText.text = "Data harvested: " + objectiveCurrent + "/" + objectiveTarget;
        if (playerState == PlayerState.PLAYING)
        {
            if (objectiveTarget > 0)
            {
                if (objectiveCurrent >= objectiveTarget)
                {
                    playerState = PlayerState.WIN;
                }
            }
        }
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
        // TODO Implement
        switch (playerState)
        {
            case PlayerState.WIN:
                Debug.Log("A WINNER IS YOU!");
                break;
            case PlayerState.PLAYING:
                Debug.Log("You are now playing.");
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
        }
    }
}