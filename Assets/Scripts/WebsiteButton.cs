using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WebsiteButton : MonoBehaviour {
    public UnityEvent clickButton;
    // Start is called before the first frame update
    void Start() {
        if (clickButton == null)
            clickButton = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
