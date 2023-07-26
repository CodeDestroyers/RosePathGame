using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FMOD.Studio;
using UnityEngine.UIElements;
using TarodevController;
using System;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anime;
    private SpriteRenderer fliper;
    private float jumpCutMultiplier = 2f;
    public float rotated;
    private float dirX = 0f;
    private EventInstance playerFootsteps;
    private bool isJumping;
    private bool isFalling;
    private float jumpTimeCounter;
    private float coyoteCounter;
    private bool wasJamp;

    [SerializeField] private float coyoteTime;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float jumpTime = 0.5f;
    






    private enum MovementState { idle, running, jumping, falling}

    private void Start()
    {
       rb = GetComponent<Rigidbody2D>();
       anime = GetComponent<Animator>();
       fliper = GetComponent<SpriteRenderer>();
       coll = GetComponent<BoxCollider2D>();



        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootsteps);
    }
    private void Update()
    {
        Move();
        Jump(); 
        UpdateAnimationState();
        UptadeSound();
        Debug.Log(isJumping);
    }
    #region Jump
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);

    }
    private void Jump()
    {
        if (IsGrounded())
        {
            coyoteCounter = coyoteTime;
            wasJamp = false;
        }
        else if (wasJamp == false)
        {
            isFalling = true;
            coyoteCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && coyoteCounter > 0 && (wasJamp == false))
        { 
            
            isJumping = true;
            coyoteCounter = 0;
            jumpTimeCounter = jumpTime;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            wasJamp = true;
        }

        if (Input.GetButton("Jump"))
        {
            if (jumpTimeCounter > 0 && isJumping)
            {
                wasJamp = true;
                isJumping = true;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
        if (rb.velocity.y < 0 && wasJamp)
        {
            rb.gravityScale = Mathf.Lerp(2.5f, 4f, 0.5f);
        }
        else
        {
            rb.gravityScale = 2.5f;
        }

    }
    #endregion

    public void Move()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
    }
    public void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            fliper.flipX = false;
            state = MovementState.running;
           // transform.rotation = Quaternion.Euler(0, 0f, 0);



        }
        else if (dirX < 0f)
        {
          
            state = MovementState.running;
            fliper.flipX = true;

            //transform.rotation = Quaternion.Euler(0, 180f, 0);

        }
        else
        {

            state = MovementState.idle;

        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;

        }

        anime.SetInteger("state", (int)state);

    }
    private void UptadeSound()
    {
        if (rb.velocity.x != 0 && IsGrounded())
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
    

}