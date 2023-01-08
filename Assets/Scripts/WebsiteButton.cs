using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WebsiteButton : MonoBehaviour
{
    public ParticleSystem myParticles;
    public GameObject myVisuals;

    public bool clickableOnce = false;
    private bool clickable;

    public UnityEvent clickButton;

    // Start is called before the first frame update
    void Start()
    {
        clickable = true;
        if (clickButton == null)
            clickButton = new UnityEvent();
    }

    [ContextMenu("Request Click")]
    public void RequestClick()
    {
        if (clickable)
        {
            clickButton.Invoke();
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
    }
}