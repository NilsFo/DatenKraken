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

    private List<GameObject> characterList;

    private void Awake()
    {
        characterList = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
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
            if (c.ToString().Trim().Length == 0) {
                previousChar = null;
                continue;
            }

            GameObject newCharacterObj = Instantiate(textCharacterPrefab, transform);
            characterList.Add(newCharacterObj);
            newCharacterObj.transform.localPosition = new Vector3(i * characterOffset, 0, 0);

            InteractableTextCharacter textCharacter = newCharacterObj.GetComponent<InteractableTextCharacter>();
            textCharacter.SetChar(c);

            if (previousChar != null) {
                textCharacter.MakeAdjacent(previousChar);
            }
            previousChar = textCharacter;
        }
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
            Handles.Label(transform.position, "Text Spawner: '" + textToDisplay + "'",style);
        }
#endif
    }
}