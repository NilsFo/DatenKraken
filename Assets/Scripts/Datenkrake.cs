using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Datenkrake : MonoBehaviour {
    public float acceleration = 0.1f;
    public float dampening = 0.1f;
    public float maxSpeed = 10f;
    
    public float tentakelAcceleration = 0.15f;
    public float pullSpeed = 20f;

    public TentacleInteraction tentakel;
    private Rigidbody2D _tentakelRB;

    public enum KrakenState {
        WALKING, GRABBING, PULLING
    }
    public KrakenState state = KrakenState.WALKING;
    public GameState gameState;

    private Vector2 _velocity;
    public Collider2D currentAdBox;

    private float _z;

    // Start is called before the first frame update
    void Start() {
        _tentakelRB = tentakel.GetComponent<Rigidbody2D>();
        gameState = FindObjectOfType<GameState>();
        
        bool success = FindAd(transform.position, out var col);
        if (success) {
            SetAdBox(col);
        } else {
            Debug.LogError("Datenkrake was not placed on an Ad on start!");
        }

        _z = transform.position.z;
    }

    // Update is called once per frame
    void Update() {
        float x = 0, y = 0;
        Vector3 krakeDeltaV = Vector3.zero;
        Vector3 tentakelDeltaV = Vector3.zero;
        
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null) {
            if (keyboard.aKey.isPressed) { x -= 1; }
            if (keyboard.dKey.isPressed) { x += 1; }

            if (keyboard.wKey.isPressed) { y += 1; }
            if (keyboard.sKey.isPressed) { y -= 1; }
            
            if (keyboard.shiftKey.wasPressedThisFrame) {
                if (state == KrakenState.WALKING) {
                    StartGrabbing();

                }
            }
            if (keyboard.shiftKey.wasReleasedThisFrame) {
                if (state == KrakenState.GRABBING) {
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
        bool success = FindInteractible(tentakel.transform.position, out var btn);
        if (success) {
            btn.clickButton.Invoke();
            Debug.Log("Interacted with button", btn);
        }
        return success;
    }
    private void StartGrabbing() {
        _tentakelRB.AddForce(new Vector2(Random.Range(-1f,1f), Random.Range(-1f,1f)).normalized * 200f);
        state = KrakenState.GRABBING;
    }
    private bool TryPull() {
        if (FindAd(tentakel.transform.position, out var col)) {
            if (col != currentAdBox) {
                state = KrakenState.PULLING;
                SetAdBox(col);
                return true;
            }
        }
        return false;
    }
    private void CancelTentakel() {

        state = KrakenState.WALKING;
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
    
    private bool FindInteractible(Vector2 pos, out WebsiteButton btn) {
        List<Collider2D> results = new List<Collider2D>();
        var contactFilter2D = new ContactFilter2D {
            layerMask = LayerMask.GetMask("Interactibles"),
            useLayerMask = true,
            useTriggers = true
        };
        var count = Physics2D.OverlapPoint(pos, contactFilter2D, results);
        if (count > 0) {
            btn = results[0].GetComponent<WebsiteButton>();
            return true;
        } else {
            btn = null;
            return false;
        }
    }
    private void PullDatenkrakeToTentakel() {
        transform.position = Vector3.MoveTowards(transform.position, tentakel.transform.position, pullSpeed * Time.deltaTime);
        if ((transform.position - tentakel.transform.position).magnitude < 0.1f) {
            state = KrakenState.WALKING;
            _velocity = transform.position - tentakel.transform.position.normalized * acceleration;
        }
    }
    private void MoveTentakel(Vector2 deltaV) {
        var tentakelPos = _tentakelRB.position;
        _tentakelRB.MovePosition(tentakelPos + deltaV * tentakelAcceleration);
        if (deltaV.sqrMagnitude > 0) {
            //tentakel.MoveRotation(Vector2.Angle(Vector2.right, deltaV));
        }
    }

    private void MoveDatenkrake(Vector2 deltaV) {

        _velocity += deltaV * acceleration;
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
}
