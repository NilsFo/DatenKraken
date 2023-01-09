using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialListener : MonoBehaviour
{
    public bool listening = true;
    public UnityEvent onPlayerEnter;

    public List<GameObject> objectsToShow;
    public List<GameObject> objectsToHide;
    public List<Advertisement> adsToShow;
    public List<WebsiteButton> webButtons;

    // Start is called before the first frame update
    void Start()
    {
        if (onPlayerEnter != null)
        {
            onPlayerEnter = new UnityEvent();
        }

        foreach (GameObject o in objectsToShow)
        {
            o.SetActive(false);
        }
        
        foreach (var websiteButton in webButtons)
        {
            websiteButton.DisableButton();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (listening)
        {
            print("Interaction?");
            TentacleInteraction tentacleInteraction = col.gameObject.GetComponent<TentacleInteraction>();

            if (tentacleInteraction != null)
            {
                onPlayerEnter.Invoke();
                foreach (GameObject o in objectsToShow)
                {
                    o.SetActive(true);
                }

                foreach (var advertisement in adsToShow)
                {
                    advertisement.DisplayAd();
                }

                foreach (var websiteButton in webButtons)
                {
                    websiteButton.EnableButton();
                    websiteButton.DisplayPoof();
                }

                foreach (var ob in objectsToHide)
                {
                    ob.SetActive(false);
                }

                listening = false;
            }
            else
            {
                print("no");
            }
        }
    }
}