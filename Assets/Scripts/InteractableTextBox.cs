using System;
using System.Collections;
using System.Collections.Generic;
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
        foreach (char c in textToDisplay.ToCharArray())
        {
            i++;
            if (c.ToString().Trim().Length==0)
            {
                continue;
            }
            
            GameObject newCharacterObj = Instantiate(textCharacterPrefab, transform);
            characterList.Add(newCharacterObj);
            newCharacterObj.transform.localPosition = new Vector3(i * characterOffset, 0, 0);

            InteractableTextCharacter textCharacter = newCharacterObj.GetComponent<InteractableTextCharacter>();
            textCharacter.SetChar(c);
        }
    }
}