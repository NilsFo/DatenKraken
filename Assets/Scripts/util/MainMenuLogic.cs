using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLogic : MonoBehaviour
{

    public GameObject MusicManagerPrefab;
    
    public Slider volumeSlider;
    private MusicManager _musicManager;

    // Start is called before the first frame update
    void Start()
    {
        _musicManager = FindObjectOfType<MusicManager>();
        if (_musicManager == null)
        {
            Instantiate(MusicManagerPrefab);
        }
        _musicManager = FindObjectOfType<MusicManager>();
        
        volumeSlider.value = _musicManager.globalUserVolume;
        _musicManager.SetMusicProfile(MusicManager.MusicProfile.MAIN_MENU);
    }

    // Update is called once per frame
    void Update()
    {
        _musicManager.globalUserVolume = volumeSlider.value;
    }
}