using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class InteractableTextCharacter : MonoBehaviour
{
    public char myChar;
    public TMP_Text myText;

    public GameState gameState;

    public float cameraShakeMagnitude;
    public float cameraShakeDuration;

    public float collectSpeed = 10f;

    public float animInterval = 3f;
    private float _animTimer;

    private bool _collected;

    public List<InteractableTextCharacter> adjacentChars;

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
            var collectionPos = gameState.player.transform.position;
            transform.position = Vector2.MoveTowards(transform.position, collectionPos, collectSpeed * Time.deltaTime);
            collectSpeed += Time.deltaTime * 100;
            if (((Vector2)collectionPos - (Vector2)transform.position).magnitude < 0.1f) {
                gameState.player.Eat();
                Destroy(gameObject);
            }

            // Setting Music
            gameState.musicManager.RequestTemporaryBoostTrommeln(1.1337f, skipFadeIn: false);
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
        if(_collected)
            return;
        
        Debug.Log("Collected: '" + myChar + "'!", gameObject);
        //Destroy(gameObject);
        _collected = true;

        Invoke(nameof(CollectAdjacent), 0.05f);
    }

    private void OnDestroy()
    {
        gameState.IncreaseObjectiveProgress();
        gameState.ShakeCamera(cameraShakeMagnitude, cameraShakeDuration);
    }

    private void CollectAdjacent() {
        foreach (var c in adjacentChars) {
            if(c != null && !c._collected)
                c.Collect();
        }
    }

    public void MakeAdjacent(InteractableTextCharacter c) {
        if (!adjacentChars.Contains(c)) {
            adjacentChars.Add(c);
            if (!c.adjacentChars.Contains(this)) {
                c.adjacentChars.Add(this);
            }
        }
    }
}