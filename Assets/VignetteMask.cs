using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VignetteMask : MonoBehaviour {
    public float transitionTime = 1f;
    public bool introOnAwake = true;
    public float introDelay = 0.5f;
    public SpriteRenderer vignetteSpriteRenderer;
    private float _transitionTimer;

    public float inScale = 0.1f, outScale = 50f;


    public enum Mode {
        INTRO, OUTRO, IDLE
    }
    public Mode mode = Mode.IDLE;
    private SpriteMask _spriteMask;
    // Start is called before the first frame update
    void Start() {
        _spriteMask = GetComponent<SpriteMask>();

        if(introOnAwake)
            PlayIntro();
    }

    // Update is called once per frame
    void Update() {
        if (mode == Mode.INTRO && _transitionTimer < transitionTime) {
            _transitionTimer += Time.deltaTime;
            var s = Mathf.Lerp(inScale, outScale, _transitionTimer / transitionTime);
            transform.localScale = new Vector3(s,s,1);
            if (_transitionTimer >= transitionTime) {
                mode = Mode.IDLE;
                vignetteSpriteRenderer.enabled = false;
            }
        }
        if (mode == Mode.OUTRO && _transitionTimer < transitionTime) {
            _transitionTimer += Time.deltaTime;
            var s = Mathf.Lerp(outScale, inScale, _transitionTimer / transitionTime);
            transform.localScale = new Vector3(s,s,1);
            if (_transitionTimer >= transitionTime) {
                mode = Mode.IDLE;
                _spriteMask.enabled = false;
            }
        }
    }

    public void PlayIntro() {
        PlayIntro(introDelay);
    }
    public void PlayIntro(float delay) {
        mode = Mode.IDLE;
        vignetteSpriteRenderer.enabled = true;
        _spriteMask.enabled = false;
        Invoke(nameof(StartIntro), delay);
    }

    private void StartIntro() {
        mode = Mode.INTRO;
        _transitionTimer = 0;
        _spriteMask.enabled = true;
    }

    public void PlayOutro() {
        mode = Mode.OUTRO;
        _transitionTimer = 0;
        vignetteSpriteRenderer.enabled = true;
        _spriteMask.enabled = true;
    }
}
