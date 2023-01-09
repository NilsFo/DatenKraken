using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Datenkrake : MonoBehaviour {
    public float acceleration = 0.1f;
    public float dampening = 0.1f;
    public float maxSpeed = 10f;
    
    public float tentakelSpeed = 0.15f;
    public float pullSpeed = 20f;

    public TentacleInteraction tentakel;
    private Rigidbody2D _tentakelRB;
    private Vector2 originalPosition;

    public enum KrakenState {
        WALKING, GRABBING, PULLING
    }
    public KrakenState state = KrakenState.WALKING;
    public GameState gameState;
    public GameObject krakenPoofPrefab;

    private Vector2 _velocity;
    public Collider2D currentAdBox;

    private float _z;
    private Camera _camera;

    private Vector2 _pullDir;

    public SpriteRenderer faceSpriteRenderer;
    private Sprite _defaultFaceSprite;
    public Sprite eatFaceSprite;
    public float eatFaceTime = 0.1f;

    private float _eatTimer;

    private void OnEnable()
    {
        originalPosition = transform.position;
    }

    // Start is called before the first frame update
    void Start() {
        _camera = Camera.main;
        _tentakelRB = tentakel.GetComponent<Rigidbody2D>();
        gameState = FindObjectOfType<GameState>();
        
        bool success = FindAd(transform.position, out var col);
        if (success) {
            SetAdBox(col);
        } else {
            Debug.LogError("Datenkrake was not placed on an Ad on start!");
        }

        _z = transform.position.z;

        _defaultFaceSprite = faceSpriteRenderer.sprite;
    }

    // Update is called once per frame
    void Update() {
        
        // eat stuff
        if (_eatTimer > 0) {
            _eatTimer -= Time.deltaTime;
            if (_eatTimer <= 0) {
                faceSpriteRenderer.sprite = _defaultFaceSprite;
            }
        }

        // happy when win
        if (gameState.playerState == GameState.PlayerState.WIN)
        {
            faceSpriteRenderer.sprite = eatFaceSprite;
        }

        float x = 0, y = 0;
        Vector3 krakeDeltaV = Vector3.zero;
        Vector3 tentakelDeltaV = Vector3.zero;
        
        // TODO tentakel z behind me
        
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null) {
            if (keyboard.aKey.isPressed) { x -= 1; }
            if (keyboard.dKey.isPressed) { x += 1; }

            if (keyboard.wKey.isPressed) { y += 1; }
            if (keyboard.sKey.isPressed) { y -= 1; }
            
            if (keyboard.shiftKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame) {
                if (state == KrakenState.WALKING) {
                    StartGrabbing();
                }
            }
            if (keyboard.shiftKey.wasReleasedThisFrame || keyboard.spaceKey.wasReleasedThisFrame || Mouse.current.leftButton.wasReleasedThisFrame) {
                if (state == KrakenState.GRABBING && gameState.ControlsEnabled()) {
                    // Try Interact
                    bool interacted = TryInteract();
                    if(interacted)
                        CancelTentakel();
                    else {
                        // Try pulling
                        var canPull = TryPull();
                        if (!canPull)
                            CancelTentakel();
                    }
                }
            }
        }

        
        
        if (state == KrakenState.WALKING && gameState.ControlsEnabled()) {
            krakeDeltaV = new Vector3(x, y, 0).normalized;
        }

        if (state == KrakenState.GRABBING && gameState.ControlsEnabled()) {
            tentakelDeltaV = new Vector3(x, y, 0).normalized;
        }

        if (state != KrakenState.PULLING) {
            MoveDatenkrake(krakeDeltaV);
            MoveTentakel(tentakelDeltaV);
        } else {
            PullDatenkrakeToTentakel();
            MoveTentakel(Vector2.zero);
        }
    }
    private bool TryInteract() {
        bool success = FindButton(tentakel.transform.position, out var btn);
        if (success) {
            btn.RequestClick();
            Debug.Log("Interacted with button", btn);
        }
        return success;
    }
    private void StartGrabbing() {
        _tentakelRB.AddForce(new Vector2(Random.Range(-1f,1f), Random.Range(-1f,1f)).normalized * 200f);
        state = KrakenState.GRABBING;
        var tentakelmover = tentakel.GetComponentInParent<TantakelMover>();
        tentakelmover.active = false;
        tentakelmover.target.parent = currentAdBox.transform;
    }
    private bool TryPull() {
        if (FindAd(tentakel.transform.position, out var col)) {
            //if (col != currentAdBox) {
                state = KrakenState.PULLING;
                _pullDir = tentakel.transform.position - transform.position;
                SetAdBox(col);
                return true;
            //}
        }
        return false;
    }
    private void CancelTentakel() {

        state = KrakenState.WALKING;
        var tentakelmover = tentakel.GetComponentInParent<TantakelMover>();
        tentakelmover.active = true;
        tentakelmover.target.parent = currentAdBox.transform;
    }

    public bool FindAd(Vector2 pos, out Collider2D col2D) {
        List<Collider2D> results = new List<Collider2D>();
        var contactFilter2D = new ContactFilter2D {
            layerMask = LayerMask.GetMask("Ad"),
            useLayerMask = true
        };
        var count = Physics2D.OverlapPoint(pos, contactFilter2D, results);
        if (count > 0) {
            col2D = results [0];
            return true;
        } else {
            col2D = null;
            return false;
        }
    }
    
    private bool FindButton(Vector2 pos, out WebsiteButton btn) {
        List<Collider2D> results = new List<Collider2D>();
        var contactFilter2D = new ContactFilter2D {
            layerMask = LayerMask.GetMask("Interactibles"),
            useLayerMask = true,
            useTriggers = true
        };
        var count = Physics2D.OverlapPoint(pos, contactFilter2D, results);
        if (count > 0) {
            foreach (var result in results) {
                var b = result.GetComponent<WebsiteButton>();
                if (b != null) {
                    btn = b;
                    return true;
                }
            }
        }
        btn = null;
        return false;
    }
    private void PullDatenkrakeToTentakel() {
        transform.position = Vector3.MoveTowards(transform.position, tentakel.transform.position, pullSpeed * Time.deltaTime);
        if ((transform.position - tentakel.transform.position).magnitude < 0.2f) {
            CancelTentakel();
            _velocity = _pullDir.normalized * acceleration;
        }
    }
    private void MoveTentakel(Vector2 deltaV) {
        var tentakelPos = _tentakelRB.position;
        if (Mouse.current.leftButton.isPressed && state != KrakenState.PULLING && gameState.ControlsEnabled()) {
            Vector2 targetPos = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 krakenPos = transform.position;
            var d = (targetPos - krakenPos).magnitude;
            if(d > 5f)
                targetPos = (targetPos - krakenPos).normalized * 5f + krakenPos;
            _tentakelRB.MovePosition(Vector2.MoveTowards(tentakelPos, targetPos, Time.deltaTime * tentakelSpeed));
        } else {
            _tentakelRB.MovePosition(tentakelPos + deltaV * (tentakelSpeed * Time.deltaTime));
            if (deltaV.sqrMagnitude > 0) {
                //tentakel.MoveRotation(Vector2.Angle(Vector2.right, deltaV));
            }
        }
    }

    private void MoveDatenkrake(Vector2 deltaV) {

        _velocity += deltaV * acceleration * Time.deltaTime;
        _velocity = DampenXY(_velocity, dampening);

        float speedSqrd = _velocity.sqrMagnitude;
        float maxSpeedSqrd = maxSpeed * maxSpeed;
        if (speedSqrd > maxSpeedSqrd) {
            _velocity.x *= Mathf.Sqrt(maxSpeedSqrd / speedSqrd);
            _velocity.y *= Mathf.Sqrt(maxSpeedSqrd / speedSqrd);
        }

        //move
        var newPosition = transform.position + (Vector3)_velocity * Time.deltaTime;
        if (!currentAdBox.OverlapPoint(newPosition)) {
            bool canSwitch = FindAd(newPosition, out var col);
            if (canSwitch) {
                SetAdBox(col);
            } else {
                var blockPosition = currentAdBox.ClosestPoint(newPosition);
                var d = blockPosition - (Vector2)newPosition;

                _velocity = _velocity - d;
                newPosition = blockPosition;
                newPosition.z = _z;
            }
        }
        transform.position = newPosition;
    }

    private Vector3 DampenXY(Vector3 vec, float pDampening) {
        if (vec.magnitude == 0)
            return vec;
        Vector3 damp = vec.normalized;
        damp.z = 0;
        //damp.Normalize();
        damp *= -pDampening * Time.deltaTime;
        if (damp.magnitude > vec.magnitude) {
            vec.x = 0;
            vec.y = 0;
        } else {
            vec += damp;
        }
        return vec;
    }
    
    private void SetAdBox(Collider2D col) {
        currentAdBox = col;
        transform.parent = col.transform;
    }

    public Vector2 GetVelocity() {
        return _velocity;
    }

    public void OnAdvertismentHidden()
    {
        if(currentAdBox == null || currentAdBox.GetComponent<Advertisement>().adEnabled == false)
        {
            Debug.Log("The player was in an ad, that has recently been closed!",gameObject);
            gameState.SoftResetLevel();
        }
    }

    public void OnSoftReset()
    {
        // TODO will this work?
        transform.position = originalPosition;
        if(state != KrakenState.WALKING)
            CancelTentakel();
        
        gameState.ShakeCamera(0.8f,0.7f);
    }

    public void Eat() {
        _eatTimer = eatFaceTime;
        faceSpriteRenderer.sprite = eatFaceSprite;
    }

    public void OnAdEnter()
    {
        //TODO call me <3
        gameState.ShakeCamera(0.1f,0.15f);
        DisplayPoof();
    }

    public void OnAdLeave()
    {
        //TODO call me <3
        gameState.ShakeCamera(0.1f,0.15f);
        DisplayPoof();
    }
    
    [ContextMenu("Poof")]
    public void DisplayPoof()
    {
        var pos = transform.position;
        var poof = Instantiate(krakenPoofPrefab);
        pos.z = pos.z + 1f;
        poof.transform.position = pos;
    }
    
    
}
