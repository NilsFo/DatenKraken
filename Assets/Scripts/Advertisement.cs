using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Advertisement : MonoBehaviour {
    public bool enabledOnStart = true;
    public bool enabled = true;
    
    public Collider2D collider;
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
        enabled = true;
        collider.enabled = enabled;
        visualization.gameObject.SetActive(enabled);
    }

    public void HideAd() {
        enabled = false;
        collider.enabled = enabled;
        visualization.gameObject.SetActive(enabled);
    }
}
