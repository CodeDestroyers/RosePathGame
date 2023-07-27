using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FMOD.Studio;
using UnityEngine.UIElements;
using TarodevController;
using System;
using JetBrains.Annotations;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    #region variables
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    public SpriteRenderer fliper;
    public float rotated;
    public Vector2 dirX;
    public Vector2 dirY;
    private EventInstance playerFootsteps;
    private PlayerControls playerControls;

    public int ZeroState;
    public bool isJumping;
    public bool isFalling;
    public bool isRunning;
    public bool isIdle;

    private float coyoteCounter;
    private bool wasJamp;
    private bool whileAttack;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    [SerializeField] private float coyoteTime;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] public float jumpForce = 2.5f;
    [SerializeField] public float jumpTime = 0.5f;
    [SerializeField] public float jumpMaxHight;   
    [SerializeField] public float jumpInterpolationSpeed;
    #endregion

    #region MainMethods
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fliper = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootsteps);
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
        
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        Idle();
        UptadeSound();
        Move();
        Jump();

    }
    private void FixedUpdate()
    {

    }
    #endregion

    #region AxisMovement
    public void Move()
    {
        dirX = playerControls.PlayerActions.Movement.ReadValue<Vector2>();

        rb.velocity = new Vector2(dirX.x * moveSpeed, rb.velocity.y);

        if (dirX.x != 0 && isFalling == false && isJumping == false)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        if (dirX.x < 0f)
        {
            fliper.flipX = true;
        }
        if (dirX.x > 0f)
        {
            fliper.flipX = false;
        }
    }

    public void Idle()
    {
        if (dirX.x == 0f && dirX.y == 0f && isFalling == false && isJumping == false && ZeroState == 0)
        {
            isIdle = true;
        }
        else
        {
            isIdle = false;
        }

    }
    #endregion

    #region Jump
    public bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);

    }
    
    private void Jump()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (IsGrounded())
        {
            coyoteCounter = coyoteTime;
            wasJamp = false;
            isFalling = false;
            isJumping = false;
            rb.gravityScale = 2.5f;
            
        }
        else if (IsGrounded() == false && wasJamp == false)
        {
            coyoteCounter -= Time.deltaTime;

        }

        if (!isJumping && !IsGrounded())
        {
            isFalling = true;
        }


        if (keyboard.spaceKey.isPressed && coyoteCounter > 0 && !wasJamp)
        {
            wasJamp = true;
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Lerp(jumpForce, jumpMaxHight, jumpInterpolationSpeed));
       
        }

        if (isFalling)
        {
            rb.gravityScale = Mathf.Lerp(2.5f, 4.5f, 0.5f);
        }

    }
    #endregion

    #region SoundPlayer
    private void UptadeSound()
    {
        if (rb.velocity.x != 0 && IsGrounded() && isRunning)
        {
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerFootsteps.start();
            }
        }
        else
        {
            playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
    #endregion

}