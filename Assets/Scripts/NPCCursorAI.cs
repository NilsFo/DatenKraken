using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NPCCursorAI : MonoBehaviour
{
    public enum AIState
    {
        LOOKING_FOR_BUTTON,
        MOVING_TO_BUTTON,
        WAITING_TO_CLICK,
        CLICKING
    }

    public AIState state;
    private AIState _lastknownState;
    public GameState _gameState;

    [Header("Visuals")] public SpriteRenderer mySpriteRenderer;
    public Sprite spriteDefault;
    public Sprite spriteClick;

    [Header("AI")] public float lookingForButtonDelay = 2.0f;
    private float _lookingForButtonTimer = 0;
    public float clickDelay = 1.337f;
    private float _clickingDelayTimer = 0f;
    public float clickAnimationDuration = 0.69f;
    private float _clickAnimationTimer = 0f;
    public GameObject lockedOnCloseBT;
    public float movementSpeed = 0.0f;

    private void Awake()
    {
        _gameState = FindObjectOfType<GameState>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _gameState.cursor = this;
        state = AIState.LOOKING_FOR_BUTTON;
        _lastknownState = state;
        ResetLookingForButtonTimer();
    }

    // Update is called once per frame
    void Update()
    {
        // Updating states: Looking for button
        if (state == AIState.LOOKING_FOR_BUTTON)
        {
            _lookingForButtonTimer += Time.deltaTime;
            if (_lookingForButtonTimer >= lookingForButtonDelay)
            {
                ResetLookingForButtonTimer();
                FindNextCLoseBT();
            }
        }
        else
        {
            ResetLookingForButtonTimer();
        }

        // Updating states: Moving
        if (state == AIState.MOVING_TO_BUTTON)
        {
            float velocity = movementSpeed * Time.deltaTime;
            float dist = Vector2.Distance(transform.position, lockedOnCloseBT.transform.position);

            if (dist < 0.1)
            {
                // Arrived
                state = AIState.WAITING_TO_CLICK;
            }
            else
            {
                Vector2 movedPos =
                    Vector2.MoveTowards(transform.position, lockedOnCloseBT.transform.position, velocity);
                Vector3 newPos = new Vector3();
                newPos.x = movedPos.x;
                newPos.y = movedPos.y;
                newPos.z = transform.position.z;
                transform.position = newPos;
            }
        }

        // Updating states: Waiting to click
        if (state == AIState.WAITING_TO_CLICK)
        {
            _clickingDelayTimer += Time.deltaTime;
            if (_clickingDelayTimer >= clickDelay)
            {
                StartClickAnimation();
            }
        }
        else
        {
            _clickingDelayTimer = 0;
        }

        // Updating states: Click Animation
        if (state == AIState.CLICKING)
        {
            _clickAnimationTimer += Time.deltaTime;
            if (_clickAnimationTimer >= clickAnimationDuration)
            {
                // Animation over. Looking for next target.
                state = AIState.LOOKING_FOR_BUTTON;
            }
        }
        else
        {
            _clickAnimationTimer = 0;
        }

        Sprite usedSprite = spriteDefault;
        if (IsClickingState())
        {
            usedSprite = spriteClick;
        }

        mySpriteRenderer.sprite = usedSprite;

        // Checking if state has changed
        if (_lastknownState != state)
        {
            _lastknownState = state;
            OnStateChanged();
        }
    }

    public void StartClickAnimation()
    {
        state = AIState.CLICKING;
    }

    [ContextMenu("Perform Click")]
    public void PerformClick()
    {
        print("NPC: CLICK!");

        if (lockedOnCloseBT != null)
        {
            AdvertismentCloseButton closeButton = lockedOnCloseBT.GetComponent<AdvertismentCloseButton>();
            if (closeButton != null)
            {
                closeButton.OnNPCClick();
            }
        }
    }

    public bool IsClickingState()
    {
        return state == AIState.CLICKING;
    }

    private void OnStateChanged()
    {
        ResetLookingForButtonTimer();
        _clickAnimationTimer = 0;
        _clickingDelayTimer = 0;

        AdvertismentCloseButton closeButton = null;
        if (lockedOnCloseBT != null)
        {
            closeButton = lockedOnCloseBT.GetComponent<AdvertismentCloseButton>();
        }

        switch (state)
        {
            case AIState.CLICKING:
                PerformClick();
                if (closeButton != null)
                {
                    closeButton.state = AdvertismentCloseButton.ClickState.CLICKED;
                }

                break;
            case AIState.MOVING_TO_BUTTON:
                if (closeButton != null)
                {
                    closeButton.state = AdvertismentCloseButton.ClickState.DEFAULT;
                }

                break;
            case AIState.WAITING_TO_CLICK:
                if (closeButton != null)
                {
                    closeButton.state = AdvertismentCloseButton.ClickState.HOVER;
                }

                break;
            case AIState.LOOKING_FOR_BUTTON:
            default:
                Debug.LogWarning("UNKNOWN AI CURSOR STATE!");
                break;
        }

        print("Cursor state: " + state);
    }

    private void FindNextCLoseBT()
    {
        print("Cursor: Looking for next Button to close!");
        List<AdvertismentCloseButton> foundButtons = new List<AdvertismentCloseButton>();

        AdvertismentCloseButton[] buttons = FindObjectsOfType<AdvertismentCloseButton>();
        if (buttons != null)
        {
            foundButtons.AddRange(buttons);
        }

        GameObject closestBT = null;
        float bestDistance = float.MaxValue;
        foreach (AdvertismentCloseButton closeButton in foundButtons)
        {
            var currentDist = Vector2.Distance(transform.position, closeButton.transform.position);
            if (currentDist < bestDistance && closeButton.myVisuals.activeSelf)
            {
                bestDistance = currentDist;
                closestBT = closeButton.gameObject;
            }
        }

        if (closestBT == null)
        {
            print("Cursor failed. We'll find them next time!");
            ResetLookingForButtonTimer();
        }
        else
        {
            lockedOnCloseBT = closestBT;
            state = AIState.MOVING_TO_BUTTON;
        }
    }

    public void ResetLookingForButtonTimer()
    {
        _lookingForButtonTimer = 0;
    }

    public void RequestInterrupt()
    {
        if (state == AIState.MOVING_TO_BUTTON)
        {
            ResetLookingForButtonTimer();
            state = AIState.LOOKING_FOR_BUTTON;
        }
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            //Debug.DrawLine(sourceNPC.transform.position, roamingOrigin);
            Vector3 wireOrigin = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 15;
            Handles.Label(transform.position, "State: " + state, style);
        }
#endif
    }
}