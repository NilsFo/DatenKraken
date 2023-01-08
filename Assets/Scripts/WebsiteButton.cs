using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WebsiteButton : MonoBehaviour
{
    public SpriteRenderer myRenderer;
    public Sprite spriteDefault;
    public Sprite spriteClicked;
    public float clickedTime = 0.69f;
    private float _clickTimeProgress = 0;

    public AudioSource buttonClickSound;

    public ParticleSystem myParticles;
    public GameObject myVisuals;
    private GameState _gameState;

    public bool clickableOnce = false;
    private bool clickable;

    public UnityEvent clickButton;

    // Start is called before the first frame update
    void Start()
    {
        _gameState = FindObjectOfType<GameState>();
        clickable = true;
        if (clickButton == null)
            clickButton = new UnityEvent();
    }

    [ContextMenu("Request Click")]
    public void RequestClick()
    {
        if (clickable)
        {
            _clickTimeProgress = clickedTime;
            clickButton.Invoke();
            _gameState.ShakeCamera(0.15f, 0.1337f);
            buttonClickSound.Play();
            if (clickableOnce)
            {
                DisableButton();
            }
        }
        else
        {
            Debug.LogWarning("Click denied!");
        }
    }

    public void DisableButton()
    {
        clickable = false;
        myParticles.Stop();
        myVisuals.SetActive(false);
    }

    public void EnableButton()
    {
        clickable = true;
        myParticles.Play();
        myVisuals.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        _clickTimeProgress -= Time.deltaTime;

        Sprite selectedSprite = spriteDefault;
        if (_clickTimeProgress > 0)
        {
            selectedSprite = spriteClicked;
        }

        myRenderer.sprite = selectedSprite;
    }
}