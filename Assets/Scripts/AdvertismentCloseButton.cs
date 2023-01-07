using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AdvertismentCloseButton : MonoBehaviour
{
    public Advertisement myAdvertisement;
    public GameObject myVisuals;
    private GameState _gameState;

    public enum ClickState
    {
        DEFAULT,
        HOVER,
        CLICKED
    }

    public ClickState state;
    public SpriteRenderer mySpriteRenderer;
    public Sprite spriteDefault;
    public Sprite spriteHover;
    public Sprite spriteClicked;

    private void Awake()
    {
        _gameState = FindObjectOfType<GameState>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetState();
    }

    // Update is called once per frame
    void Update()
    {
        Sprite usedSprite = spriteDefault;
        switch (state)
        {
            case ClickState.HOVER:
                usedSprite = spriteHover;
                break;
            case ClickState.CLICKED:
                usedSprite = spriteClicked;
                break;
        }

        mySpriteRenderer.sprite = usedSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TentacleInteraction tentacleInteraction = other.gameObject.GetComponent<TentacleInteraction>();
        if (tentacleInteraction != null)
        {
            print("Collision with the player!");
            _gameState.ShakeCamera(.7f, 1.337f);
        }

        NPCCursorAI cursor = other.gameObject.GetComponent<NPCCursorAI>();
        if (tentacleInteraction != null)
        {
            print("Collision with the NPC!");
            OnNPCClick();
        }
    }

    [ContextMenu("NPC Click")]
    public void OnNPCClick()
    {
        print("The NPC clicked me!");
        Close();
    }

    public void Close()
    {
        myAdvertisement.HideAd();
    }

    public void OnShowAd()
    {
        myVisuals.SetActive(true);
        NotifyGameState();
    }

    public void OnHideAd()
    {
        myVisuals.SetActive(false);
    }

    public void NotifyGameState()
    {
        _gameState.OnCloseableAdAppeared(this);
    }

    public void ResetState()
    {
        state = ClickState.DEFAULT;
    }
}