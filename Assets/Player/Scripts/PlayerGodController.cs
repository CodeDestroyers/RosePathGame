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
using UnityEngine.TextCore.Text;
using Cinemachine;
using UnityEngine.SceneManagement;

public class PlayerGodController : MonoBehaviour, IDataPersistence
{
    #region SaveLoad Methods

    public void LoadData(GameData data)
    {
        this.playerMaxHp = data.playerMaxHP;
        this.playerCurrentHp = data.playerCurrentHp;
        this.playerRespawnPoint = data.playerRespawnScene;
    }

    public void SaveData(ref GameData data)
    {
        data.playerCurrentHp = this.playerCurrentHp;
        data.playerMaxHP = this.playerMaxHp;
    }

    #endregion

    #region Variables

    //For _HP and Getting damage
    public int playerCurrentHp;
    public int playerMaxHp = 200;
    [SerializeField] private float playerInvulnerabilityTime;
    [SerializeField] private float playerCollisionInvulnerabilityTime;
    [SerializeField] private float collisionPower;
    public SpriteRenderer playerSprite;
    private Vector2 damageDirection;
    private int staticCoolisionDamage = 20;
    public bool playerWasHit;
    public bool playerWasCollisionHit;

    //For _Movement
    public static Vector2 dirX;
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
    private bool wasJump;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float baseDrag;

    //For _Attacks

    private bool attackEnd = false;
    private Vector2 horizontalAttackVector;
    private float attackCooldownTime;
    [SerializeField] private float attackStartTime;
    [SerializeField] private Transform attackPos;
    public float enumAttackTime;
    public float upwardsForce;
    public float defaultForce;
    public PlayerAttackScript playerAttackScript;

    //For objects
    private BoxCollider2D coll;
    private CircleCollider2D cColl;

    //For Clasess
    public PlayerControls playerControls;

    //For Animator 

    [SerializeField] private Animator animator;
    private string CurrentState;

    //Movement States
    private int SPECIAL_STATE;
    private int MODE_STATE;
    private int ATTACK_STATE;
    private bool player_isIdle = false;
    private bool player_isRun = false;
    private bool player_isJump = false;
    private bool player_isFall = false;
    private bool player_isClimbing = false;
    private bool player_isClimbingJump = false;
    private bool player_isRunStart = false;
    private bool player_WasFall = false;

    //Attack States

    private bool duringAttack;
    private bool player_isForwardAttack;
    private bool player_isForwardAttackRun;
    private bool player_isForwardAttackFall;
    private bool player_isUpwardAttackIdle;
    private bool player_isUpwardAttackRun;
    private bool player_isUpwardAttackFall;
    private bool player_isDownwardAttackFall;

    //For Layers
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask jumpableGround;

    //For Collision Check
    private RaycastHit2D GroundHit;
    private Transform m_Transform;
    [SerializeField]  private float distanceRay;


    //For Camera Control

    private CinemachineImpulseSource impulseSource;

    //For sound play

    private bool wasAttackSound;
    private EventInstance playerFootsteps;
    private EventInstance playerJumpSound;

    //For Player Load

    public int collisionPosition;
    public static int scenePosition = 1;
    private GameObject loadPosition;
    private GameObject collisionLoadPosition;
    public int collisionPoint = 1;
    public bool wasCollisionHitWall = false;

    //For Bonfire

    public int playerLastBonfire;
    public string playerRespawnPoint;


    #endregion

    #region MethodsMain

    private void PlayerSceneLoader()
    {
        if (scenePosition == 1)
        {
            loadPosition = GameObject.FindWithTag("LoadPosition1");
        }

        else if (scenePosition == 2)
        {
            loadPosition = GameObject.FindWithTag("LoadPosition2");
        }

        else if(scenePosition == 3)
        {
            loadPosition = GameObject.FindWithTag("LoadPosition3");
        }
        else if (scenePosition == 4)
        {
            loadPosition = GameObject.FindWithTag("LoadPosition4");
        }
        else if (scenePosition == 5)
        {
            loadPosition = GameObject.FindWithTag("LoadPosition5");
        }
        else if (scenePosition == 6)
        {
            loadPosition = GameObject.FindWithTag("LoadPosition6");
        }
        else if (scenePosition == 7)
        {
            loadPosition = GameObject.FindWithTag("LoadPosition7");
        }

        else if (scenePosition == 8)
        {
            loadPosition = GameObject.FindWithTag("Bonfire");
        }

        transform.localPosition = loadPosition.transform.localPosition;
    }


    private void Start()
    {
        DataPersistenceManager.Instance.LoadGame();
        impulseSource = GetComponent<CinemachineImpulseSource>();

        //For sound getters
        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootsteps);
        playerJumpSound = AudioManager.instance.CreateInstance(FMODEvents.instance.playerJump);
    }

    private void Awake()
    {
        PlayerSceneLoader();
        m_Transform = GetComponent<Transform>();
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        cColl = GetComponent<CircleCollider2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerControls.PlayerActions.ChooseVerticalDerection.performed += ctx => horizontalAttackVector = ctx.ReadValue<Vector2>();
        playerAttackScript = GetComponentInChildren<PlayerAttackScript>();
        playerCurrentHp = playerMaxHp;
        

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
        CollisionDamage();
        playerWasCollisionHitColl();
        IsAir();
        StateMachine();
        AttackState();
        PlayerMove();
        PlayerMovementAnimator();
        PlayerAttackAnimator();

        PlayerDeath();

        UpdateSound();

        Debug.Log("Current State: " + CurrentState);
        Debug.Log("MODE STATE:" + MODE_STATE);
        Debug.Log("ATTACK STATE?: " + ATTACK_STATE);
        Debug.Log("Was Collision hit? " + playerWasCollisionHit);
        Debug.Log("Player current hp " + playerCurrentHp);

    }

    private void FixedUpdate()
    {
        playerCollisionDamageForce();
    }
    #endregion

    #region MethodsStateCheck

    //Invoke By Attack Animation Event
    public void AttackEnd()
    {
        duringAttack = false;
        ATTACK_STATE = 0;
        attackEnd = true;
        wasAttackSound = false;
    }

    private void AttackState() 
    {
        if (playerControls.PlayerActions.AttackL.WasPressedThisFrame() && !attackEnd)
        {
            player_WasFall = false;
            ATTACK_STATE = 1;
            player_isFall = false;
            player_isIdle = false;
            player_isJump = false;
            player_isRun = false;
            player_isRunStart = false;
        }
        if (attackEnd == true)
        {
            attackEnd = false;
            ATTACK_STATE = 0;
            player_isUpwardAttackFall = false;
            player_isUpwardAttackIdle = false;
            player_isUpwardAttackRun = false;
            player_isDownwardAttackFall = false;
            player_isForwardAttackFall = false;
            player_isForwardAttack = false;
            player_isForwardAttackRun = false;
            animator.StopPlayback();
}
    }

    private bool IsWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool IsGround()
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

    public void fallEnd()
    {
        player_WasFall = false;
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
            AirStates();
        }

        if (IsWall() && !IsAir())
        {
            MODE_STATE = 3;
        }
    }
    #endregion

    #region ModeState1 (Ground)

    private void GroundStates()
    {
        rb.drag = baseDrag;

        if (MODE_STATE == 1)
        {
            coyoteCounter = coyoteTime;
            jumpTimeCounter = jumpTime;

            //Previous State Changes

            player_isFall = false;

            if (ATTACK_STATE == 1 && !duringAttack)
            {
                PlayerAttackState1();
           
            }
            else
            {
                flip();

            if (SPECIAL_STATE == 1)
            {
                   //Some Special States like Door open animation and etc.
            }
            if (ATTACK_STATE == 0)
                {
                    moveSpeed = 4f;

                    if (dirX.x != 0 && !player_WasFall)
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
            wasJump = true;
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


    private void PlayerAttackState1()
    {

        //Use Berserk Method and Combo there later (MAYBE)

        if (horizontalAttackVector.y >= 0.1f)
        {
            ATTACK_STATE = 1;

            if (rb.velocity.x == 0f)
            {
               player_isUpwardAttackIdle = true;
                duringAttack = true;
            }

            if (rb.velocity.x != 0f)
            {
                player_isUpwardAttackRun = true;
                duringAttack = true;
            }
        }

        if (horizontalAttackVector.y <= 0f)
        {
            ATTACK_STATE = 1;

            if (rb.velocity.x == 0f)
            {
                player_isForwardAttack = true;
                duringAttack = true;
            }

            if (playerControls.PlayerActions.Movement.IsPressed())
            {
                player_isForwardAttackRun = true;
                duringAttack = true;            
            }
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

    #region ModeState2 (Air)

    private void AirStates()
    {
        if (MODE_STATE == 2)
        {

            //Previous State Changes

            player_isIdle = false;
            player_isRun = false;
            player_isRunStart = false;
            player_WasFall = true;

            if (ATTACK_STATE == 1 && !duringAttack)
            {
                PlayerAttackState2();
            }
            else if (ATTACK_STATE == 0)
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

            if (jumpTimeCounter <= 0)
            {
                wasJump = true;
            }
            if (jumpTimeCounter > 0)
            {
                wasJump = false;
            }

            if (wasJump)
            {
                rb.drag = fallSpeed;
            }

            else if (!wasJump && jumpTimeCounter == jumpTime)
            {
                rb.drag = fallSpeed;
            } 

            else if (!wasJump)
            {
                rb.drag = baseDrag;
            }
        }
    }

    private void PlayerAttackState2()
    {
        if (horizontalAttackVector.y == 0f)
        {
            player_isForwardAttackFall = true;
            rb.drag = 15;
            duringAttack = true;
        }

        if (horizontalAttackVector.y  == -1f)
        {
            player_isDownwardAttackFall = true;
            duringAttack = true;
        }

        if (horizontalAttackVector.y == 1f)
        {
            player_isUpwardAttackFall = true;
            duringAttack = true;
        }
    }

    #endregion

    #region ModeState3 (Wall)
    #endregion

    #region MethodsAttackAndDamage

    public void HandleMovement()
    {
        //Checks to see if the GameObject should allow the player to move when melee attack colides
        if (playerAttackScript.collided)
        {
            //If the attack was in a downward direction
            if (playerAttackScript.downwardStrike)
            {
                //Propels the player upwards by the amount of upwardsForce in the meleeAttackManager script
                rb.AddForce(playerAttackScript.direction * upwardsForce);
            }
            else
            {
                //Propels the player backwards by the amount of horizontalForce in the meleeAttackManager script
                rb.AddForce(playerAttackScript.direction * defaultForce);
            }
        }
    }

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

        if (player_WasFall && IsGround() && ATTACK_STATE == 0 && !player_isRun)
        {
            animator.Play("Player_AfterJump");
            CurrentState = ("After Jump");
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
            player_WasFall = false;
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

        if (player_isUpwardAttackIdle)
        {
            CurrentState = ("Attack_Upward_Idle");
            animator.Play("Attack_Upward_Idle");
            return;

        }
        if (player_isUpwardAttackRun)
        {
            CurrentState = ("Attack_Upward_Run");
            animator.Play("Attack_Upward_Run");
            return;
        }
        if (player_isUpwardAttackFall)
        {
            CurrentState = ("Attack_Upward_Jump");
            animator.Play("Attack_Upward_Jump");
            return;
        }
        if (player_isDownwardAttackFall)
        {
            CurrentState = ("Attack_Downward_Fall");
            animator.Play("Attack_Downward_Fall");
            return;

        }
        if (player_isForwardAttack)
        {
            CurrentState = ("ForwardAttack");
            animator.Play("Forward_Attack");
            return;
        }
        if (player_isForwardAttackRun)
        {
            CurrentState = ("Attack_Forward_Run");
            animator.Play("Attack_Forward_Run");
            return;
        }
        if (player_isForwardAttackFall)
        {
            CurrentState = ("Forward_Attack_Jummp");
            animator.Play("Attack_Forward_Jump");
            return;
        }

    }

    #endregion

    #region HP and damage collision METHODS


    public void PlayerDeath()
    {
        if (playerCurrentHp == 0)
        {
            SceneManager.LoadScene(playerRespawnPoint);
            playerCurrentHp = playerMaxHp;
            scenePosition = 8;
        }
    }

    public void playerWasCollisionHitColl()
    {
        if (wasCollisionHitWall)
        {
            if (collisionPoint == 1)
            {
                collisionLoadPosition = GameObject.FindWithTag("CollisionPosition1");
                transform.localPosition = collisionLoadPosition.transform.localPosition;
                wasCollisionHitWall = false;
            }
            if (collisionPoint == 2)
            {
                collisionLoadPosition = GameObject.FindWithTag("CollisionPosition2");
                transform.localPosition = collisionLoadPosition.transform.localPosition;
                wasCollisionHitWall = false;
            }
            if (collisionPoint == 3)
            {
                collisionLoadPosition = GameObject.FindWithTag("CollisionPosition3");
                transform.localPosition = collisionLoadPosition.transform.localPosition;
                wasCollisionHitWall = false;
            }
            if (collisionPoint == 4)
            {
                collisionLoadPosition = GameObject.FindWithTag("CollisionPosition4");
                transform.localPosition = collisionLoadPosition.transform.localPosition;
                wasCollisionHitWall = false;
            }
        }
    }

    public void Damage(int amount)
    {
        playerCurrentHp -= amount;
        playerWasHit = true;
        Debug.Log(playerCurrentHp);
        CameraShakeManager.instance.CameraShake(impulseSource);
        GetComponentInChildren<ParticleSystem>().Play();    

        
        if (playerCurrentHp <= 0)
        {
            //Caps currentHealth to 0 for cleaner code
            playerCurrentHp = 0;
            //Removes GameObject from the scene; this should probably play a dying animation in a method that would handle all the other death logic, but for the test it just disables it from the sce
        }
        else
        {
            //Coroutine that runs to allow the enemy to receive damage again
            StartCoroutine(TurnOffHit());
        }
    }

    private void CollisionDamage()
    {

        if (wasCollisionHitWall)
        {
            GetComponentInChildren<ParticleSystem>().Play();
            PlayerCollisionDamageSound();
            playerCurrentHp -= staticCoolisionDamage;
            playerSprite.color = Color.Lerp(Color.white, Color.black, 0.5f);
            StartCoroutine(CollisionOffHit());
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !playerWasCollisionHit || collision.gameObject.CompareTag("DamageCollision") && !playerWasCollisionHit)
        {
            PlayerCollisionDamageSound();

            if (collision.gameObject.transform.position.x > transform.position.x)
            {
                damageDirection = new Vector2 (-1f, 0.1f);
            }

            else if (collision.gameObject.transform.position.x < transform.position.x)
            {
                damageDirection = new Vector2(1f, 0.1f);
            }
            playerWasCollisionHit = true;

            playerSprite.color = Color.Lerp(Color.white, Color.black, 0.5f);

            playerCurrentHp -= staticCoolisionDamage;

            CameraShakeManager.instance.CameraShake(impulseSource);

            StartCoroutine(CollisionOffHit());

            GetComponentInChildren<ParticleSystem>().Play();

        }
    }

    private void playerCollisionDamageForce()
    {
        if (playerWasCollisionHit)
        {
            rb.AddForce(damageDirection * collisionPower);
        }
    }

    private IEnumerator TurnOffHit()
    {
        playerWasHit = false;
        yield return new WaitForSeconds(playerInvulnerabilityTime);
        playerSprite.color = Color.gray;
    }
    private IEnumerator CollisionOffHit()
    {
        playerWasCollisionHit = false;
        yield return new WaitForSeconds(playerCollisionInvulnerabilityTime);
        playerSprite.color = Color.gray;
    }
    #endregion

    #region SoundPlayer
    private void UpdateSound()
    {
        FootStepSound();
        JumpSound();
        AttackSound();
    }

    private void FootStepSound()
    {
        if (player_isRun)
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

    private void JumpSound()
    {
        if (playerControls.PlayerActions.Jump.WasPressedThisFrame() && player_isJump)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerJump, this.transform.position);
        }
    }

    private void AttackSound()
    {
        if (playerControls.PlayerActions.AttackL.WasPressedThisFrame() && !wasAttackSound)
        {
            wasAttackSound = true;
           AudioManager.instance.PlayOneShot(FMODEvents.instance.baseAttack, this.transform.position);
        }
    }

    private void PlayerCollisionDamageSound()
    {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerTakeHit, this.transform.position);
    }

    #endregion

}
