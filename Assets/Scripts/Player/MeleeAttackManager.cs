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
    public bool meleeAttack;
    //The animator on the meleePrefab
    private Animator meleeAnimator;

    public Vector2 moveVal;
    private bool wasAttack;

    private PlayerControls playerControls;
    private MeleeWeapon damage;

    private bool airAttack;
    public int comboState;

    public bool isUpwardAttackIdle;
    public bool isUpwardAttackJump;
    public bool isUpwardAttackRun;
    public bool isDownwardAttack;
    public bool isForwardAttack;
    public bool isForwardAttackRun;
    public bool isForwardAttackJump;
    public bool isForwardComboOne;
    public bool isForwardComboTwo;

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
        damage = GetComponent <MeleeWeapon>();
        //The Animator component on the player
        anim = GetComponent<Animator>();
        //The Character script on the player; this script on my project manages the grounded state, so if you have a different script for that reference that script
        character = GetComponent<PlayerMovement>();
        //The animator on the meleePrefab
        meleeAnimator = GetComponentInChildren<MeleeWeapon>().gameObject.GetComponent<Animator>();


        OnEnable();
        AttackEnd();
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
        if (playerControls.PlayerActions.AttackL.WasPressedThisFrame() && character.MovementState < 3)
        {
            //Sets the meleeAttack bool to true
            meleeAttack = true;

            //Checks to see if meleeAttack is true and pressing up
            if (meleeAttack && moveVal.y > 0 && character.AttackState == 0 && character.isRunning)
            {
                character.AttackState = 1;
                isUpwardAttackRun = true;
            }
            else
            {
                isUpwardAttackRun = false;

            }

            if (meleeAttack && moveVal.y > 0 && character.AttackState == 0 && character.isJumping || isFalling)
            {
                character.AttackState = 1;
                isUpwardAttackJump = true;
                airAttack = true;
            }
            else
            {
                isUpwardAttackJump = false;

            }

            if (meleeAttack && moveVal.y > 0 && character.AttackState == 0 && character.isIdle)
            {
                isUpwardAttackIdle = true;
                character.AttackState = 1;
            }
            else
            {
                isUpwardAttackIdle = false;

            }

            if (meleeAttack && moveVal.y > 0 && character.AttackState == 0 && character.isJumping || isFalling)
            {
                isDownwardAttack = true;
                character.AttackState = 1;
                airAttack = true;

            }
            else
            {
                isDownwardAttack = false;
            }


            //Checks to see if meleeAttack is true and pressing down while also not grounded
            if (meleeAttack && moveVal.y < 0 && !character.IsGrounded() && character.AttackState == 0)
            {
                isDownwardAttack = true;
                character.AttackState = 1;
                airAttack = true;
            }
            else
            {
                isDownwardAttack = false;
            }


            #region comboForward

            //Checks to see if meleeAttack is true and not pressing any direction
            if ((meleeAttack && moveVal.y == 0 && character.AttackState == 0 && comboState == 0)
                //OR if melee attack is true and pressing down while grounded
                || meleeAttack && moveVal.y < 0 && character.IsGrounded())
            {
                if (character.isRunning)
                {
                    character.AttackState = 1;
                    isForwardAttack = false;
                    isForwardAttackRun = true;
                    return;
                }
                else
                {
                    isForwardAttackRun = false;
                }


                if (character.MovementState == 0 && !isForwardAttackRun)
                {
                    character.AttackState = 1;
                    isForwardAttack = true;
                    isForwardAttackRun = false;
                    damage.damageAmount = 35;
                }

                else
                {
                    isForwardAttack = false;
                }

                if (character.isFalling || character.isJumping)
                {
                    character.AttackState = 1;
                    isForwardAttackJump = true;
                    airAttack = true;
                    damage.damageAmount = 25;
                }

                else
                {
                    isForwardAttackJump = false;
                }

            }

          
            #endregion

        }

        if (isForwardAttack && comboState == 1 && berserkMode)
        {
            if (playerControls.PlayerActions.AttackL.WasPressedThisFrame())
            {
                isForwardComboOne = true;
                isForwardAttack = false;
                damage.damageAmount = 50;
            }
        }

        if (isForwardComboOne && comboState == 2 && berserkMode)
        {
            if (playerControls.PlayerActions.AttackL.WasPressedThisFrame())
            {
                damage.damageAmount = 100;
                isForwardComboTwo = true;
                isForwardComboOne = false;
            }
        }



    }

    public void comboStateOne()
    {
        comboState = 1;
    }
    public void comboStateTwo()
    {
        comboState = 2;
    }

    public void AttackEnd()
    {
        isDownwardAttack = false;
        isUpwardAttackIdle = false;
        isUpwardAttackJump = false;
        isUpwardAttackRun = false;
        isForwardAttackRun = false;
        isForwardAttack = false;
        isForwardAttackJump = false;
        meleeAttack = false;
        airAttack = false;
        isForwardComboOne = false;
        isForwardComboTwo = false;
        comboState = 0;
        character.AttackState = 0;
    }

    #endregion
}
