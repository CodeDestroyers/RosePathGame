using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anime;
    private SpriteRenderer fliper;

    [SerializeField] private LayerMask jumpableGround; 

    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 11.5f;

    private enum MovementState { idle, running, jumping, falling}

    private void Start()
    {
       rb = GetComponent<Rigidbody2D>();
       anime = GetComponent<Animator>();
       fliper = GetComponent<SpriteRenderer>();
       coll = GetComponent<BoxCollider2D>();
    }

    
    private void Update()
    {

        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        }

        UpdateAnimationState();
    }



    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            fliper.flipX = false;
            state = MovementState.running;
        }
        else if (dirX < 0f)
        {
            fliper.flipX = true;
            state = MovementState.running;
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
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

}