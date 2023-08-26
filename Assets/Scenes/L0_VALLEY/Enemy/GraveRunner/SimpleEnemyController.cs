using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Drawing;

public class SimpleEnemyController : MonoBehaviour
{
    #region Variable

    //Nubers Variables For Methods
    private int damageAmount = 40;
    [SerializeField] private float MovementsSpeed;
    private Animator anim;

    //For Collision
    public int positionOfPatrol;
    public Transform pointLeft;
    public Transform pointRight;
    private GameObject player;
    private Rigidbody2D rb;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private float maxDistanceDelta;
    [SerializeField] private float idleTime;

    //States
    public bool goRight;
    private int ENEMY_ATTACK_STATE;
    private bool isIdle;
    private bool isPatrol;
    private bool roseInAttackPresence;
    private bool isAttack;
    private bool isBack;
    public bool goLeft;
    private bool isChasing;
    private bool moveingRight;

    #endregion

    private EnemyHealth health;
    #region BaseMainMethods

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        goLeft = true;
        goRight = false;
        isIdle = false;
        pointLeft = GetComponent<Transform>();
        pointRight = GetComponent<Transform>();

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Enemy go left?" + goLeft);
        Debug.Log("Enemy go right?" + goRight);
        Debug.Log("Enemy is idle?" + isIdle);
    }

    private void FixedUpdate()
    {
        StateMachine();
    }

    #endregion

    #region DetectMethods
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Checks to see if the GameObject the MeleeWeapon is colliding with has an EnemyHealth script
        if (collision.GetComponent<PlayerGodController>())
        {
            //Method that checks to see what force can be applied to the player when melee attacking
            DamagePlayer(collision.GetComponent<PlayerGodController>());

        }
    }

    private void DamagePlayer(PlayerGodController objHealth)
    {
        objHealth.Damage(damageAmount);
    }

    #endregion

    #region EnemyStateMachine

    private void StateMachine()
    {
        MovementMethods();
    }

    #endregion

    #region MovementMethods

    private void MovementMethods()
    {
        if (!health.hit)
        {
            enemyPatrol();
        }
    }

    private void enemyIdle()
    {
        anim.Play("Boss1_idle");
    }
    private void enemyPatrol()
    {
        if (goLeft && !isIdle)
        {
            rb.velocity = new Vector2(-MovementsSpeed, 0f);
            transform.localScale = new Vector2(-1, 1);
            anim.Play("Boss1_run");

        }

        if (goRight && !isIdle)
        {
            rb.velocity = new Vector2(MovementsSpeed, 0f);
            transform.localScale = new Vector2(1, 1);
            anim.Play("Boss1_run");
        }

        /*if (pointLeft.transform.position.x == transform.localPosition.x && goLeft)
        {
            isIdle = true;
            StartCoroutine(enemySwitchIdleRight());
        }
        if (pointRight.transform.position.x == transform.localPosition.x && goRight)
        {
            isIdle = true;
            StartCoroutine(enemySwitchIdleLeft());
        }*/
    }

    private IEnumerator enemySwitchIdleRight()
    {
        enemyIdle();
        yield return new WaitForSeconds(idleTime);
        goRight = true;
        goLeft = false;
        isIdle = false;

    }
    private IEnumerator enemySwitchIdleLeft()
    {
        enemyIdle();
        yield return new WaitForSeconds(idleTime);
        goRight = false;
        goLeft = true;
        isIdle = false;

    }

    #endregion
}