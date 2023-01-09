using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class VideoMidrollPlayer : MonoBehaviour
{
    public float adTime = 15.0f;
    public float skipBTVisibleAt = 5.0f;

    private float adTimeProgress = 0;
    public Advertisement midRollAd;
    public Advertisement bannerAd;
    public TMP_Text adOverText;
    public WebsiteButton myWebsiteButton;
    public GameObject adOverHolder;

    private bool showingMidRoll;

    private void Awake()
    {
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
        adOverHolder.SetActive(false);
        if (showingMidRoll)
        {
            adTimeProgress += Time.deltaTime;
            adOverText.text = "Video plays in " + GetFormattedCountdown() + "s";

            if (adTimeProgress > skipBTVisibleAt)
            {
                adOverHolder.SetActive(true);
            }

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
        midRollAd.ShowAd();
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