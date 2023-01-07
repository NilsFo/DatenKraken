using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InteractableTextCharacter : MonoBehaviour
{
    public char myChar;
    public TMP_Text myText;

    public GameState gameState;

    public float cameraShakeMagnitude;
    public float cameraShakeDuration;

    public float animInterval = 3f;
    private float _animTimer;

    private bool _collected;

    private void Awake()
    {
        SetChar('?');
    }

    // Start is called before the first frame update
    void Start()
    {
        gameState = FindObjectOfType<GameState>();
        gameState.IncreaseObjectiveTarget();
        _animTimer = Random.Range(0f, animInterval);
    }

    // Update is called once per frame
    void Update() {
        if (_collected) {
            var tentaclepos = gameState.player.tentakel.transform.position;
            transform.position = Vector2.MoveTowards(transform.position, tentaclepos, 10 * Time.deltaTime);
            if (((Vector2)tentaclepos - (Vector2)transform.position).magnitude < 0.1f) {
                Destroy(gameObject);
            }
        }
        _animTimer += Time.deltaTime;
        if (_animTimer > animInterval)
            _animTimer -= animInterval;
        var x = Mathf.Sin(_animTimer / animInterval * Mathf.PI * 2) * 4f;
        var y = Mathf.Cos(_animTimer / animInterval * 2 * Mathf.PI * 2) * 4f;
        myText.transform.localPosition = new Vector3(x, y, 0);
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
        //Destroy(gameObject);
        _collected = true;
    }

    private void OnDestroy()
    {
        gameState.IncreaseObjectiveProgress();
    }
}