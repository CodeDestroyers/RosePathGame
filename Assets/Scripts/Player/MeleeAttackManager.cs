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

    private PlayerControls playerControls;

    public bool isUpwardAttack;
    public bool isDownwardAttack;
    public bool isForwardAttack;

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
            character.ZeroState = 1;
        }
        else
        {
            //Turns off the meleeAttack bool
            meleeAttack = false;
            character.ZeroState = 0;
        }
        //Checks to see if meleeAttack is true and pressing up
        if (meleeAttack && moveVal.y > 0)
        {
            isUpwardAttack = true;
            character.ZeroState = 1;
        }
        //Checks to see if meleeAttack is true and pressing down while also not grounded
        if (meleeAttack && moveVal.y < 0 && !character.IsGrounded())
        {
            isDownwardAttack = true;
            character.ZeroState = 1;
        }
        //Checks to see if meleeAttack is true and not pressing any direction
        if ((meleeAttack && moveVal.y == 0)
             //OR if melee attack is true and pressing down while grounded
            || meleeAttack && moveVal.y < 0 && character.IsGrounded())
        {
            isForwardAttack = true;
            character.ZeroState = 1;

        }
    }
    #endregion
}
