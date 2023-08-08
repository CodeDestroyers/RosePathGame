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
using static UnityEditor.Searcher.SearcherWindow.Alignment;

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
    public bool isOnWall;

    private bool wasCrowling;
    private float switcherCrowling;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    private float _fallSpeedYDampingChangeTreshold;
    private bool _wasBabyJamp;

    [SerializeField] public float moveSpeed = 4f;
    public BoxCollider2D flipCollision;

    [Header("Climbing Settings")]

    public bool isWallSliding;
    private float wallSlidingSpeed = 2f;

    public bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);
    private bool isFacingRight = true;


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
            WallSlide();
            WallJump();
            Debug.Log("Is Wall Jumping? " + isWallJumping);
            Debug.Log("Is Grounded? " + IsGrounded());
            Debug.Log("Movement state: " + MovementState);
            Debug.Log("Is walled? " + IsWalled());


            if (!isWallJumping)
            {
                Flip();
            }

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
        if (!isWallJumping)
        {
            rb.velocity = new Vector2(dirX.x * moveSpeed, rb.velocity.y);
        }
    
    }
    #endregion

    #region MainMovementMethods
    private void Move()
    {
        if (AttackState == 0)
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

            else if (isFalling || isJumping)
            {
                isRunning = false;
            }

            if (MovementState == 0)
            {
                isWallJumping = false;
            }
        }

        /*if (dirX.x < 0f && !isWallJumping)
        {
            gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
            coll.offset = new Vector2(-0.15f, -0.04311925f);
            


        }
        if (dirX.x > 0f && !isWallJumping)
        {
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            coll.offset = new Vector2(0.15f, -0.04311925f);
            
            
        }*/
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

        if (!IsGrounded() && !isJumping && !isWallJumping && !isWallSliding && MovementState < 3)
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

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && dirX.x != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (playerControls.PlayerActions.Jump.WasPressedThisFrame() && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= 1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && dirX.x < 0f || !isFacingRight && dirX.x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
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