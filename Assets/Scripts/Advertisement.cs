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

    public Collider2D adCollider;
    public Transform visualization;

    public UnityEvent onHideAd;
    public UnityEvent onShowAd;

    private void OnEnable()
    {
        if (onHideAd == null) onHideAd = new UnityEvent();
        if (onShowAd == null) onShowAd = new UnityEvent();
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
            HideAd();
        }

        ChooseNextImg();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChooseNextImg()
    {
        // TODO Cycle random images?
    }

    public void DisplayAd()
    {
        adEnabled = true;
        adCollider.enabled = adEnabled;
        visualization.gameObject.SetActive(adEnabled);
        ChooseNextImg();

        onShowAd.Invoke();
    }

    public void HideAd()
    {
        adEnabled = false;
        adCollider.enabled = adEnabled;
        visualization.gameObject.SetActive(adEnabled);

        onHideAd.Invoke();
    }
}