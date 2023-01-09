using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InteractableTextBox : MonoBehaviour
{
    public GameObject textCharacterPrefab;

    [Header("Text Settings")] public string textToDisplay;

    [Header("Character Settings")] public float characterOffset = 0.8f;
    private float lastKnownOffset;

    private GameState _gameState;
    private List<GameObject> characterList;
    public int charactersEaten;
    private bool stingerPlayed;

    private void Awake()
    {
        characterList = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        stingerPlayed = false;
        _gameState = FindObjectOfType<GameState>();
        BuildCharacters();
        lastKnownOffset = characterOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastKnownOffset != characterOffset)
        {
            BuildCharacters();
            lastKnownOffset = characterOffset;
        }
    }

    private void LateUpdate()
    {
        if (!stingerPlayed && charactersEaten == characterList.Count)
        {
            print("Whole box has been eaten!");
            stingerPlayed = true;

            if (_gameState.playerState != GameState.PlayerState.WIN)
            {
                _gameState.stingerGood.Play();
            }
        }
    }

    [ContextMenu("Rebuild")]
    public void BuildCharacters()
    {
        foreach (GameObject o in characterList)
        {
            Destroy(o);
        }

        characterList = new List<GameObject>();
        int i = -1;
        InteractableTextCharacter previousChar = null;
        foreach (char c in textToDisplay.ToCharArray())
        {
            i++;
            if (c.ToString().Trim().Length == 0)
            {
                previousChar = null;
                continue;
            }

            GameObject newCharacterObj = Instantiate(textCharacterPrefab, transform);
            characterList.Add(newCharacterObj);
            newCharacterObj.transform.localPosition = new Vector3(i * characterOffset, 0, 0);

            InteractableTextCharacter textCharacter = newCharacterObj.GetComponent<InteractableTextCharacter>();
            textCharacter.SetChar(c);
            textCharacter.myParentTextBox = this;

            if (previousChar != null)
            {
                textCharacter.MakeAdjacent(previousChar);
            }

            previousChar = textCharacter;
        }
    }

    public void ReportEaten()
    {
        charactersEaten++;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Vector3 wireOrigin = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.black;
            style.fontSize = 15;

            Handles.DrawWireDisc(wireOrigin, Vector3.forward, 0.5f);
            Handles.Label(transform.position, "Text Spawner: '" + textToDisplay + "'", style);
        }
#endif
    }
}