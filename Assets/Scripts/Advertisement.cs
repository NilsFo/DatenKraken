using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Advertisement : MonoBehaviour {
    public bool enabledOnStart = true;
    public bool adEnabled = true;
    
    public Collider2D adCollider;
    public Transform visualization;
    
    // Start is called before the first frame update
    void Start()
    {
        if (enabledOnStart) {
            DisplayAd();
        } else {
            HideAd();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayAd() {
        adEnabled = true;
        adCollider.enabled = adEnabled;
        visualization.gameObject.SetActive(adEnabled);
    }

    public void HideAd() {
        adEnabled = false;
        adCollider.enabled = adEnabled;
        visualization.gameObject.SetActive(adEnabled);
    }
}
