using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class VideoMidrollPlayer : MonoBehaviour
{
    public float adTime = 5.0f;
    private float adTimeProgress = 0;
    public Advertisement midRollAd;
    public Advertisement bannerAd;
    public TMP_Text adOverText;
    public WebsiteButton myWebsiteButton;
    private GameObject adOverHolder;

    private bool showingMidRoll;

    private void Awake()
    {
        adOverHolder = adOverText.gameObject;
        showingMidRoll = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        adOverHolder.SetActive(false);
        adTimeProgress = 0;
    }

    // Update is called once per frame
    void Update()
    {
        adOverHolder.SetActive(showingMidRoll);
        if (showingMidRoll)
        {
            adTimeProgress += Time.deltaTime;
            adOverText.text = "Video plays in " + GetFormattedCountdown() + "s";

            if (adTimeProgress > adTime)
            {
                OnTimeExpired();
            }
        }
        else
        {
            adTimeProgress = 0;
        }
    }

    public int GetFormattedCountdown()
    {
        float remainingTime = adTime - adTimeProgress;
        return (int)remainingTime;
    }

    public void OnTimeExpired()
    {
        adTimeProgress = 0;
        showingMidRoll = false;
        myWebsiteButton.EnableButton();
        midRollAd.HideAd();
        bannerAd.DisplayAd();
    }

    public void OnCookieClick()
    {
        adTimeProgress = 0;
        showingMidRoll = true;

        myWebsiteButton.DisableButton();
        midRollAd.DisplayAd();
        bannerAd.HideAd();
    }

    public void OnShowMidRoll()
    {
    }

    public void OnHideMidRoll()
    {
    }

    public void OnShowBanner()
    {
    }

    public void OnHideBanner()
    {
    }
}