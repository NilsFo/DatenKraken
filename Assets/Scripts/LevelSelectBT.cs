using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectBT : MonoBehaviour
{
    private Button _myButton;
    public String levelName;

    // Start is called before the first frame update
    void Start()
    {
        _myButton = GetComponentInChildren<Button>();
        _myButton.onClick.AddListener(Click);
    }

    private void Click()
    {
        SceneManager.LoadScene(levelName);
    }
}