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

public class PlayerGodController : MonoBehaviour
{
    #region Variables

    //For _OnCelling
    [SerializeField] private float cellingHight;
    private RaycastHit2D CeilingHit;

    //For _Movement
    private Vector2 dirX;
    private Vector2 dirY;
    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float baseMoveSpeed = 4f;

    //For _Jump

    [SerializeField] private float jumpTimeCounter;
    [SerializeField] private float coyoteCounter;
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpTime;

    //For _Crowling

    private float switcherCrowling;
    private bool wasCrowling;

    //For _Attacks

    private bool attackEnd = false;
    private bool attackStart = false;

    //For objects
    private BoxCollider2D coll;

    //For Clasess
    private PlayerControls playerControls;

    //For Animator 

    [SerializeField] private Animator animator;
    private string CurrentState;

    //States
    private int SPECIAL_STATE;
    private int MODE_STATE;
    private int ATTACK_STATE;
    private bool player_isIdle = false;
    private bool player_isRun = false;
    private bool player_isJump = false;
    private bool player_isFall = false;
    private bool player_isCrowlingIdle = false;
    private bool player_isCrowlingRun = false;
    private bool player_isClimbing = false;
    private bool player_isClimbingJump = false;
    private bool player_isRunStart = false;

    //For Layers
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask jumpableGround;

    //For Collision Check
    private RaycastHit2D GroundHit;

    #endregion

    #region MethodsMain
    private void Start()
    {

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

    void Update()
    {
        IsAir();
        StateMachine();
        AttackState();
        onCeiling();
        PlayerMove();
        GroundStates();
        AirStates();
        PlayerMovementAnimator();

        Debug.Log("Current State: " + CurrentState);
        Debug.Log("MODE STATE:" + MODE_STATE);
        Debug.Log("PLAYER JUMP?:" + player_isJump);
        Debug.Log("PLAYER FALL?:" + player_isFall);

    }
    #endregion

    #region MethodsStateCheck

    //Invoke By Attack Animation Event
    public void AttackEnd()
    {
        attackEnd = true;
    }

    private int AttackState() 
    {
        if (playerControls.PlayerActions.AttackL.WasPressedThisFrame() && !attackEnd && !attackStart)
        {
            attackStart = true;
            return 1;
        }
        else if (attackEnd == true)
        {
            attackStart = false;
            return 0;
        }
        else
        {
            return 2;
        }
    }


    private bool IsWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    private bool IsGround()
    {
        GroundHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);

        if (GroundHit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }


    }

    private bool IsAir()
    {
        if (!IsWall() && !IsGround())
        {
            return true;
        }
        else return false;
    }


    #endregion

    #region MethodsStateMachine

    private void StateMachine()
    {
        if (IsGround())
        {
            MODE_STATE = 1;
            GroundStates();
        }

        if (IsAir())
        {
            MODE_STATE = 2;
        }

        if (IsWall())
        {
            MODE_STATE = 3;
        }
    }
    #endregion

    #region ModeState1

    private void GroundStates()
    {
        if (MODE_STATE == 1)
        {
            coyoteCounter = coyoteTime;
            jumpTimeCounter = jumpTime;

            //Previous State Changes

            player_isFall = false;

            if (ATTACK_STATE == 1)
            {
                PlayerAttackState1();
            }
            else
            {
                flip();

                if (onCeiling())
                {
                    PlayerCrowling();
                }
                else if (SPECIAL_STATE == 1)
                {
                    player_isCrowlingIdle = false;
                    player_isCrowlingRun = false;

                    //Some Special States like Door open animation and etc.
                }
                else
                {
                    player_isCrowlingIdle = false;
                    player_isCrowlingRun = false;

                    if (dirX.x != 0)
                    {
                        player_isRun = true;
                        player_isIdle = false;
                    }
                    else if (dirY.x == 0)
                    {
                        player_isRun = false;
                        player_isRunStart = false;
                        player_isIdle = true;
                    }
                    else
                    {
                        player_isIdle = false;
                        player_isRunStart = false;
                        player_isRun = false;
                    }
                    PlayerJump();
                }
            }
        }
    }

    private void PlayerMove()
    {
        dirX = playerControls.PlayerActions.Movement.ReadValue<Vector2>();
        rb.velocity = new Vector2(dirX.x * moveSpeed, rb.velocity.y);
    }

    public void PlayerStartMove()
    {
        player_isRunStart = true;
    }

    private void PlayerJump()
    {

        if (playerControls.PlayerActions.Jump.WasPressedThisFrame() && coyoteCounter > 0 && jumpTimeCounter > 0f && !player_isJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            coyoteCounter = 0;
            player_isJump = true;
        }

        if (playerControls.PlayerActions.Jump.WasReleasedThisFrame() && player_isJump)
        {
            jumpTimeCounter = 0;
        }

        if (playerControls.PlayerActions.Jump.IsPressed())
        {
            if (jumpTimeCounter > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
                coyoteCounter = 0;
                player_isJump = true;
            }
        }

    }

    private void PlayerCrowling()
    {
        switcherCrowling = playerControls.PlayerActions.Crowling.ReadValue<float>();

        if (switcherCrowling > 0.5f)
        {
            wasCrowling = true;

            if (wasCrowling)
            {
                moveSpeed = 2f;
            }

            if (wasCrowling && dirX.x != 0)
            {
                player_isCrowlingRun = true;
                player_isCrowlingIdle = false;
            }
            if (wasCrowling && dirX.x == 0)
            {
                player_isCrowlingRun = false;
                player_isCrowlingIdle = true;
            }
        }

        if (switcherCrowling < 0.5f)
        {
            wasCrowling = false;

            if(dirX.x != 0)
            {
                player_isCrowlingIdle = false;
                player_isCrowlingRun = true;
            }
            else if (dirX.x == 0)
            {
                player_isCrowlingIdle = true;
                player_isCrowlingRun = false;
            }
        }

    }

    private void PlayerAttackState1()
    {

    }

    private bool onCeiling()
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

    private void flip()
    {
        if (dirX.x > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (dirX.x < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }


    #endregion

    #region ModeState2

    private void AirStates()
    {
        if (MODE_STATE == 2)
        {

            //Previous State Changes

            player_isIdle = false;
            player_isRun = false;
            player_isRunStart = false;

            if (ATTACK_STATE == 1)
            {
                PlayerAttackState2();
            }
            else
            {
                flip();

                if (coyoteCounter <=0 && jumpTimeCounter <=0)
                {
                    player_isJump = false;
                    player_isFall = true;
                }

                if (coyoteCounter > 0f || player_isJump)
                {
                    PlayerJump();
                }
                if (!player_isJump)
                {
                    coyoteCounter -= Time.deltaTime;
                    player_isFall = true;
                }

                else
                {
                    player_isFall = false;
                }
            }
        }
    }

    private void PlayerAttackState2()
    {

    }

    #endregion

    #region ModeState3
    #endregion

    #region MethodsAnimation

    private void PlayerMovementAnimator()
    {
        /*if (PLAYER_ISCLIMBING)
        {
            animator.Play("Player_Climbing");
            CurrentState = ("Player_isClimbing");
            return;
        }*/

        if (player_isCrowlingIdle)
        {
            animator.Play("Player_crow_idle");
            CurrentState = ("Player_isCrowling_idle");
            return;
        }

        if (player_isCrowlingRun)
        {
            animator.Play("Player_Crow_Run");
            CurrentState = ("Player_isCrowlingRun");
            return;

        }

        if (player_isIdle)
        {
            animator.Play("Player_Idle");
            CurrentState = ("Idle");
            return;
        }

        if (player_isRun && !player_isRunStart)
        {
            animator.Play("Player_Run_Start");
            CurrentState = ("Player_Run_Start");
            return;
        }

        if (player_isRun && player_isRunStart)
        {
            animator.Play("Player_Running");
            CurrentState = ("Run");
            return;
        }

        if (player_isJump)
        {
            animator.Play("Player_Jump");
            CurrentState = ("Jump");
            return;
        }

        if (player_isFall)
        {
            animator.Play("Player_Fall");
            CurrentState = ("Fall");
            return;
        }
    }

    private void PlayerAttackAnimator()
    {

    }

    #endregion
}
