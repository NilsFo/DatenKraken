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

    public float cameraShakeMagnitude;
    public float cameraShakeDuration;

    private void Awake()
    {
        SetChar('?');
    }

    // Start is called before the first frame update
    void Start()
    {
        gameState = FindObjectOfType<GameState>();
        gameState.IncreaseObjectiveTarget();
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        TentacleInteraction tentacleInteraction = other.gameObject.GetComponent<TentacleInteraction>();
        if (tentacleInteraction != null)
        {
            // print("Collision with: " + myChar);
            Collect();
        }
    }

    [ContextMenu("Collect me")]
    public void Collect()
    {
        Debug.Log("Collected: '" + myChar + "'!", gameObject);
        gameState.ShakeCamera(cameraShakeMagnitude, cameraShakeDuration);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        gameState.IncreaseObjectiveProgress();
    }
}