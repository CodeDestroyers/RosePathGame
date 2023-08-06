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

    public int AttackState;
    public int ZeroState;


    // If Movement state = 0 = is Idle
    // If Movement state = 1 = is any movement states
    // If Movement state = 2 = is climbing states
    // If Movement state = 3 = is crowling states
    public int MovementState;




    public bool isJumping;
    public bool isFalling;
    public bool isRunning;
    public bool isIdle;
    public bool isCrowlingIdle;
    public bool isCrowlingRun;

    private bool wasCrowling;
    private float switcherCrowling;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;   

    private float _fallSpeedYDampingChangeTreshold;
    private bool _wasBabyJamp;

    [SerializeField] public float moveSpeed = 4f;

    [Header("Jump Settings")]
    [SerializeField] private float attackGravity;
    [SerializeField] private float JumpTimeCounter;
    [SerializeField] private float coyoteCounter;
    [SerializeField] private float coyoteTime;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] public float jumpForce;
    [SerializeField] public float jumpTime;
    [SerializeField] public float jumpMaxHight;   
    [SerializeField] public float jumpInterpolationSpeed;
    [SerializeField][Range(0, 1)] private float shortJumpFactor;
    [SerializeField] private float cellingHight;
    private float extraHeight = 0.25f;
    private RaycastHit2D GroundHit;
    private RaycastHit2D CeilingHit;
    #endregion

    #region MainMethods
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fliper = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

        OnEnable();

        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootsteps);

        _fallSpeedYDampingChangeTreshold = CameraManager.instance._fallSpeedYDampingChangeTreshold;
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
        crowlInput();
        Idle();
        UptadeSound();
        Move();
        MovementStateCheck();
        DrawGroundCheck();
        Jump();
        rb.WakeUp();


        Debug.Log("Switcher: " + switcherCrowling);
        Debug.Log("Movement state: " + MovementState);
        Debug.Log("Attack State:" + AttackState);
        Debug.Log("Crowling State: " + isCrowlingIdle);

        #region CameraLerp

        if (rb.velocity.y < _fallSpeedYDampingChangeTreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.Instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        if (rb.velocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;

            CameraManager.instance.LerpYDamping(false);
        }
        #endregion

    }
    private void FixedUpdate()
    {
    
    }
    #endregion

    #region MainMovementMethods
    private void Move()
    {
        dirX = playerControls.PlayerActions.Movement.ReadValue<Vector2>();

        rb.velocity = new Vector2(dirX.x * moveSpeed, rb.velocity.y);

        if (dirX.x != 0 && MovementState == 0 && AttackState == 0 && IsGrounded() && !isFalling && !isJumping)
        {
            isRunning = true;
        }

        else if (dirX.x == 0)
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

    private void MovementStateCheck()
    {
        if (!isJumping && !isFalling && !isRunning && !isCrowlingIdle && !isCrowlingRun)
        {
            MovementState = 0;
        }

        if (isJumping || isFalling)
        {
            MovementState = 1;
        }

        if (isRunning)
        {
            MovementState = 1;
        }

        if (isCrowlingIdle || isCrowlingRun)
        {
            MovementState = 3;
        }
    }

    private void Idle()
    {
        if (MovementState == 0 && AttackState == 0 && IsGrounded())
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
            JumpTimeCounter = jumpTime;
            _wasBabyJamp = false;
        }
        
        if (!IsGrounded() && isFalling)
        {
            coyoteCounter -= Time.deltaTime;

        }

        if (playerControls.PlayerActions.Jump.WasPressedThisFrame() && coyoteCounter > 0 && !isJumping && MovementState < 3 && MovementState >= 0)
        {
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            coyoteCounter = 0;
        }

        if (playerControls.PlayerActions.Jump.WasReleasedThisFrame())
        {
            _wasBabyJamp = true;
        }

        if (playerControls.PlayerActions.Jump.IsPressed() && !_wasBabyJamp && MovementState < 3 && MovementState >= 0)
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

        if (!IsGrounded() && !isJumping)
        {
            isFalling = true;
        }


    }


    #endregion

    #region CrowlState


    public bool onCeiling()
    {
        CeilingHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.up, cellingHight, jumpableGround);

        if (CeilingHit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private void crowlInput()
    { 

        switcherCrowling = playerControls.PlayerActions.Crowling.ReadValue<float>();

        if (switcherCrowling > 0.5f)
        {
            if (isRunning)
            {
                MovementState = 3;
            }

            if(isIdle || isIdle && onCeiling())
            {
               isCrowlingIdle = true;
             
            }
            else if(isRunning || isRunning && onCeiling())
            {
                isCrowlingRun = true;
            }
            if (isCrowlingIdle && dirX.x != 0)
            {
                isCrowlingRun = true;
                isCrowlingIdle = false;
            }
            
            if (isCrowlingRun && dirX.x == 0)
            {
                isCrowlingIdle = true;
                isCrowlingRun = false;
            }

            if (dirX.x == 0 && onCeiling())
            {
                isCrowlingRun = false;
                isCrowlingIdle = true;
            }
            if (isCrowlingIdle && onCeiling())
            {
                isCrowlingIdle = true;
            }
            if (isCrowlingRun && dirX.x != 0 && onCeiling())
            {
                isCrowlingRun = true;
            }
        }

        else if (switcherCrowling < 0.5f)
        {
            if (!isRunning && !isIdle && !onCeiling())
            {
                isCrowlingIdle = false;
                isCrowlingRun = false;
            }
            if (isRunning && !isIdle && onCeiling())
            {
                isCrowlingIdle = false;
                isCrowlingRun = true;
            }
            if (isCrowlingRun && dirX.x != 0 && !onCeiling())
            {
                isCrowlingRun = false;
            }
            if (isCrowlingIdle && !onCeiling())
            {
                isCrowlingIdle = false;
            }
            if (dirX.x == 0 && onCeiling())
            {
                isCrowlingIdle = true;
                isCrowlingRun = false;
            }
            if (dirX.x != 0 && onCeiling())
            {
                isCrowlingIdle = false;
                isCrowlingRun = true;
            }
            if (onCeiling() && isFalling || onCeiling() && isJumping)
            {
                isCrowlingIdle = false;
                isCrowlingRun = false;
            }
        }
    }
    #endregion


    #region TriggerMethods

    public void DragIpdateDown()
    {
        rb.drag = 5;
    }
    public void DragIpdateUp()
    {
        rb.drag = 10;
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