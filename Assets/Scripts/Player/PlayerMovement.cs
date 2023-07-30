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

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;   

    [SerializeField] private float moveSpeed = 4f;

    [Header("Jump Settings")]
    [SerializeField] private float JumpTimeCounter;
    [SerializeField] private float coyoteCounter;
    [SerializeField] private float coyoteTime;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] public float jumpForce;
    [SerializeField] public float jumpTime;
    [SerializeField] public float jumpMaxHight;   
    [SerializeField] public float jumpInterpolationSpeed;
    [SerializeField][Range(0, 1)] private float shortJumpFactor;
    private float extraHeight = 0.25f;
    private RaycastHit2D GroundHit;
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
        DrawGroundCheck();
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

        if (dirX.x != 0 && isFalling == false && isJumping == false && IsGrounded())
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
        if (dirX.x == 0f && dirX.y == 0f && isFalling == false && isJumping == false && ZeroState == 0 && IsGrounded())
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
        GroundHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);

        if(GroundHit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
        

    }
    
    private void Jump()
    {
        if (IsGrounded())
        {
            coyoteCounter = coyoteTime;
            isFalling = false; 
            isJumping = false;
            rb.gravityScale = 2.5f;
            JumpTimeCounter = jumpTime;

        }
        
        if (!IsGrounded() && isFalling)
        {
            coyoteCounter -= Time.deltaTime;

        }

        if (playerControls.PlayerActions.Jump.WasPressedThisFrame() && coyoteCounter > 0)
        {
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            coyoteCounter = 0;
        }

        if (playerControls.PlayerActions.Jump.IsPressed())
        {
            if (JumpTimeCounter > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                JumpTimeCounter -= Time.deltaTime;
                coyoteCounter = 0;

            }

            else if(JumpTimeCounter == 0f)
            {
                isFalling = true;
                isJumping = false;
            }

            else
            {
                isJumping = false;
            }

        if (playerControls.PlayerActions.Jump.WasReleasedThisFrame())
            {
                isJumping = false;
            }
            

        }

        if (isFalling)
        {
            rb.gravityScale = Mathf.Lerp(2.5f, 4.5f, 0.5f);
        }

        if (!IsGrounded() && !isJumping)
        {
            isFalling = true;
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

    #region DebugFunctions

    private void DrawGroundCheck()
    {
        Color rayColor;
        
        if (IsGrounded())
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(coll.bounds.center + new Vector3(coll.bounds.extents.x, 0), Vector2.down * (coll.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(coll.bounds.center - new Vector3(coll.bounds.extents.x, 0), Vector2.down * (coll.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(coll.bounds.center - new Vector3(coll.bounds.extents.x, coll.bounds.extents.y + extraHeight), Vector2.right * (coll.bounds.extents.x * 2), rayColor);
    }

    #endregion
}