using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Advertisement : MonoBehaviour
{
    public List<Sprite> possibleSprites;
    public SpriteRenderer myRenderer;

    public bool enabledOnStart = true;
    public bool adEnabled = true;

    public float respawnTimer = -1;
    private float _respawn_progress = 0f;

    public Collider2D adCollider;
    public Transform visualization;
    public SpriteRenderer frame;
    
    public float displayAnimTime = 1f;
    public Vector2 popupSourceLocation = new Vector2(0,0);

    public UnityEvent onHideAd;
    public UnityEvent onShowAd;

    private GameState _gameState;

    private float _displayTimer;
    private Vector2 _frameSize;

    private void OnEnable()
    {
        if (onHideAd == null) onHideAd = new UnityEvent();
        if (onShowAd == null) onShowAd = new UnityEvent();
        _gameState = FindObjectOfType<GameState>();
        _frameSize = frame.size;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (enabledOnStart)
        {
            ShowAd();
        }
        else
        {
            HideAd(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _respawn_progress += Time.deltaTime;
        if (respawnTimer > 0 && _respawn_progress >= respawnTimer && !adEnabled)
        {
            DisplayAd();
        }

        if (_displayTimer > 0) {
            var f = _displayTimer / displayAnimTime;
            _displayTimer -= Time.deltaTime;
            frame.transform.position = Vector2.Lerp(transform.localToWorldMatrix.MultiplyPoint3x4(Vector2.zero),popupSourceLocation, f);
            frame.size = Vector2.Lerp(_frameSize, Vector2.zero, f);

            if (_displayTimer <= 0) {
                _displayTimer = 0;
                frame.transform.localPosition = Vector2.zero;
                myRenderer.enabled = true;
            }
        }
    }

    public void ChooseNextImg()
    {
        // TODO Cycle random images?
    }

    private void ShowAd() {
        _respawn_progress = 0;
        if (!adEnabled)
        {
            ChooseNextImg();
        }

        adEnabled = true;
        adCollider.enabled = adEnabled;
        visualization.gameObject.SetActive(adEnabled);

        onShowAd.Invoke();
    }
    public void DisplayAd() {
        // Prep animation
        popupSourceLocation = _gameState.player.tentakel.transform.position;
        _displayTimer = displayAnimTime;
        frame.transform.position = popupSourceLocation;
        frame.size = Vector2.zero;
        myRenderer.enabled = false;
        // show
        ShowAd();
    }

    public void HideAd(bool notifyGameState = true)
    {
        _respawn_progress = 0;
        adEnabled = false;
        adCollider.enabled = adEnabled;
        visualization.gameObject.SetActive(adEnabled);

        onHideAd.Invoke();

        if (notifyGameState)
        {
            // Checking for player
            if (_gameState != null)
            {
                _gameState.player.OnAdvertismentHidden();
            }
        }
    }

    public void OnSoftReset()
    {
        if (!adEnabled && enabledOnStart)
        {
            ShowAd();
        }
    }
}