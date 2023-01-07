using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InteractableTextCharacter : MonoBehaviour
{
    public char myChar;
    public TMP_Text myText;

    public GameState gameState;

    private void Awake()
    {
        SetChar('?');
    }

    // Start is called before the first frame update
    void Start()
    {
        gameState = FindObjectOfType<GameState>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetChar(char c)
    {
        myChar = c;
        myText.SetText(c.ToString());
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Collision with: " + myChar);
        TentacleInteraction tentacleInteraction = other.gameObject.GetComponent<TentacleInteraction>();
        if (tentacleInteraction != null)
        {
            print("Collision with the player tentacle!");
        }
    }

    [ContextMenu("Collect me")]
    public void Collect()
    {
        Debug.Log("Collected!", gameObject);
        Destroy(gameObject);
    }
}