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

    public UnityEvent onHideAd;
    public UnityEvent onShowAd;

    private GameState _gameState;

    private void OnEnable()
    {
        if (onHideAd == null) onHideAd = new UnityEvent();
        if (onShowAd == null) onShowAd = new UnityEvent();
        _gameState = FindObjectOfType<GameState>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (enabledOnStart)
        {
            DisplayAd();
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
    }

    public void ChooseNextImg()
    {
        // TODO Cycle random images?
    }

    public void DisplayAd()
    {
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
            DisplayAd();
        }
    }
}