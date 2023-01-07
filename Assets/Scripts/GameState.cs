using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{

    public Datenkrake player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Datenkrake>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
