using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class MeleeAttackManager : PlayerMovement
{
    #region Variables
    //How much the player should move either downwards or horizontally when melee attack collides with a GameObject that has EnemyHealth script on it
    public float defaultForce = 300;
    //How much the player should move upwards when melee attack collides with a GameObject that has EnemyHealth script on it
    public float upwardsForce = 600;
    //How long the player should move when melee attack collides with a GameObject that has EnemyHealth script on it
    public float movementTime = .1f;
    //Input detection to see if the button to perform a melee attack has been pressed
    private bool meleeAttack;
    //The animator on the meleePrefab
    private Animator meleeAnimator;

    public Vector2 moveVal;
    private bool wasAttack;

    private PlayerControls playerControls;

    public bool isUpwardAttack;
    public bool isDownwardAttack;
    public bool isForwardAttack;
    public bool isForwardAttackRun;

    //The Animator component on the player
    private Animator anim;
    //The Character script on the player; this script on my project manages the grounded state, so if you have a different script for that reference that script
    private PlayerMovement character;
    #endregion

    #region MainMethods
    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.PlayerActions.ChooseVerticalDerection.performed += ctx => moveVal = ctx.ReadValue<Vector2>();

    }

    private void Start()
    {
        //The Animator component on the player
        anim = GetComponent<Animator>();
        //The Character script on the player; this script on my project manages the grounded state, so if you have a different script for that reference that script
        character = GetComponent<PlayerMovement>();
        //The animator on the meleePrefab
        meleeAnimator = GetComponentInChildren<MeleeWeapon>().gameObject.GetComponent<Animator>();

        OnEnable();
        ForwardEnd();
    }


    private void Update()
    {
        //Method that checks to see what keys are being pressed
        CheckInput();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }



    #endregion

    #region InputMethods
    private void CheckInput()
    {


        //Checks to see if Backspace key is pressed which I define as melee attack; you can of course change this to anything you would want
        if (playerControls.PlayerActions.AttackL.WasPressedThisFrame())
        {
            //Sets the meleeAttack bool to true
            meleeAttack = true;
        }
        //Checks to see if meleeAttack is true and pressing up
        if (meleeAttack && moveVal.y > 0 && character.AttackState == 0)
        {
            isUpwardAttack = true;
            character.AttackState = 1;
        }
        else
        {
            isUpwardAttack = false;
            character.AttackState = 0;
            
        }


        //Checks to see if meleeAttack is true and pressing down while also not grounded
        if (meleeAttack && moveVal.y < 0 && !character.IsGrounded() && character.AttackState == 0)
        {
            isDownwardAttack = true;
            character.AttackState = 1;
        }
        else
        {
            isDownwardAttack = false;
            character.AttackState = 0;
        }


        //Checks to see if meleeAttack is true and not pressing any direction
        if ((meleeAttack && moveVal.y == 0 && character.AttackState == 0)
             //OR if melee attack is true and pressing down while grounded
            || meleeAttack && moveVal.y < 0 && character.IsGrounded())
        {
            if (character.isRunning)
            {
                isForwardAttack = false;
                isForwardAttackRun = true;
                character.AttackState = 1;
            }
            else if (!character.isRunning && !character.isFalling && !character.isJumping && !isForwardAttackRun && character.IsGrounded())
            {
                character.isIdle = false;
                isForwardAttack = true;
                isForwardAttackRun = false;
                character.AttackState = 1;
            }

            else
            {
                character.AttackState = 0;
            }

        }

        
    }
    public void ForwardEnd()
    {
        isForwardAttackRun = false;
        isForwardAttack = false;
        character.AttackState = 0;
        meleeAttack = false;
    }
    #endregion
}
