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

    // Start is called before the first frame update
    void Start()
    {
        _gameState = FindObjectOfType<GameState>();
    }

    // Update is called once per frame
    void Update()
    {
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

}