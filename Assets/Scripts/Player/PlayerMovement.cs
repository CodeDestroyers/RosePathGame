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
using UnityEngine.Animations;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    public SpriteRenderer fliper;
    public float rotated;
    public Vector2 dirX;
    public Vector2 dirY;
    private EventInstance playerFootsteps;
    private PlayerControls playerControls;
    private float flip;


    #region StateVariable

    // If Movement state = 0 = is Idle
    // If Movement state = 1 = is any movement states
    // If Movement state = 2 = is climbing states
    // If Movement state = 3 = is crowling states
    
    [Header("StateVariable")]
    public int AttackState;
    public int ZeroState;
    public int MovementState;
    public bool isJumping;
    public bool isFalling;
    public bool isRunning;
    public bool isIdle;
    public bool isCrowlingIdle;
    public bool isCrowlingRun;
    public bool isOnWall;

    private bool wasCrowling;
    private float switcherCrowling;
    private bool _wasBabyJamp;

    #endregion

    #region ExpenseVariable

    public int playerMoney;

    public int berserkCounter;
    private int berkserkCounterMin = 0;
    private int berserkCounterMax = 100;
    public bool berserkMode;

    #endregion

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    private float _fallSpeedYDampingChangeTreshold;

    [SerializeField] public float moveSpeed = 4f;
    public BoxCollider2D flipCollision;

    [Header("Climbing Settings")]
    private float wallJumpCooldown;
    public bool isWallSliding;



    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;


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
        fliper = GetComponent<SpriteRenderer>();
        OnEnable();

        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootsteps);

        _fallSpeedYDampingChangeTreshold = CameraManager.instance._fallSpeedYDampingChangeTreshold;
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
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
        if (Time.timeScale > 0)
        {
            crowlInput();
            Idle();
            UptadeSound();
            Move();
            MovementStateCheck();
            DrawGroundCheck();
            Jump();
            rb.WakeUp();
            Debug.Log("Movement state: " + MovementState);
            wallCheckFunc();
          

        }

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
        if (AttackState == 0)
        {
            dirX = playerControls.PlayerActions.Movement.ReadValue<Vector2>();

            rb.velocity = new Vector2(dirX.x * moveSpeed, rb.velocity.y);

            if (dirX.x > 0.01f)
            {
                transform.localScale = Vector3.one;
            }
            else if (dirX.x < -0.01f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

            if (dirX.x != 0 && MovementState == 0 && AttackState == 0 && IsGrounded() && !isFalling && !isJumping)
            {
                isRunning = true;
            }

            else if (dirX.x == 0)
            {
                isRunning = false;
            }

            else if (isFalling || isJumping)
            {
                isRunning = false;
            }
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
        if (MovementState == 0 && AttackState == 0 && IsGrounded() && !isCrowlingIdle && !isCrowlingRun && !onCeiling())
        {
            isIdle = true;
        }
        else if (onCeiling())
        {
            isIdle = false;
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

        if (!IsGrounded() && isFalling && !onWall())
        {
            coyoteCounter -= Time.deltaTime;

        }

        if (playerControls.PlayerActions.Jump.WasPressedThisFrame() && coyoteCounter > 0 && !isJumping && MovementState < 3 && MovementState >= 0 && IsGrounded())
        {
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            coyoteCounter = 0;
        }

        else if (playerControls.PlayerActions.Jump.WasPressedThisFrame()&& onWall() && !IsGrounded())
        {
            if (dirX.x == 0)
            {
                rb.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 20, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
             rb.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 6, 12);
            
            wallJumpCooldown = 0;
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

        if (!IsGrounded() && !isJumping && !isWallSliding && MovementState < 3)
        {
            isFalling = true;
        }

        if (isFalling)
        {
            rb.drag = Mathf.Lerp(rb.drag, 2f, 1f);
        }
        else
        {
            rb.drag = 5f;
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
            wasCrowling = true;
            MovementState = 3;

            if (wasCrowling)
            {
                moveSpeed = 2f;
            }

            if (wasCrowling && dirX.x != 0)
            {
                isCrowlingRun = true;
                isCrowlingIdle = false;
            }
            if (wasCrowling && dirX.x == 0)
            {
                isCrowlingRun = false;
                isCrowlingIdle = true;
            }
        }
        if (switcherCrowling < 0.5f)
        {
            wasCrowling = false;
        }
        if (dirX.x != 0 && onCeiling())
        {
            isCrowlingIdle = false;
            isCrowlingRun = true;
        }

        if (dirX.x == 0 && onCeiling())
        {
            isCrowlingIdle = true;
            isCrowlingRun = false;
        }

        if (!onCeiling() && !wasCrowling || onCeiling() && isFalling || onCeiling() && isJumping || !IsGrounded())
        {
            isCrowlingIdle = false;
            isCrowlingRun = false;
        }
        if (!onCeiling() && !wasCrowling && MovementState == 3)
        {
            moveSpeed = 3.2f;
            MovementState = 0;
        }

        if (IsGrounded() && !onCeiling() && !wasCrowling)
        {
            isCrowlingIdle = false;
            isCrowlingRun = false;
        }
    }
    #endregion

    #region ClimbingState

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    private void wallCheckFunc()
    {
        if (wallJumpCooldown > 0.2f)
        {

            if (onWall() && !IsGrounded())
            {
                isWallSliding = true;
                rb.gravityScale = 1;
                rb.velocity = Vector2.zero;
                fliper.flipX = true;
            }

            else
            {
                fliper.flipX = false;
                isWallSliding = false;
                rb.gravityScale = 3.2f;
            }
        }

        else
        {
            wallJumpCooldown += Time.deltaTime;
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