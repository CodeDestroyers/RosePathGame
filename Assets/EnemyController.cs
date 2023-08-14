using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Drawing;

public class EnemyController : MonoBehaviour
{
    #region Variable

    //Nubers Variables For Methods
    private int damageAmount = 40;
    [SerializeField] private float MovementsSpeed;
    private Animator anim;

    //For Collision
    public int positionOfPatrol;
    public Transform point;
    private GameObject player;
    private Rigidbody2D rb;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private float maxDistanceDelta;
    private float idleTimer = 4;

    //States
    private int ENEMY_ATTACK_STATE;
    private bool isIdle;
    private bool isPatrol;
    private bool roseInAttackPresence;
    private bool isAttack;
    private bool isBack;
    private bool goLeft;
    private bool goRight;
    private bool isChasing;
    private bool moveingRight;

    #endregion

    #region BaseMainMethods

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        RoseInChacePresence();
        RoseInAttackPresence();
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


    private bool RoseInChacePresence()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < stoppingDistance)
        {
            Debug.Log("Rose in presence!");
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool RoseInAttackPresence()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < attackDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region EnemyStateMachine

    private void StateMachine()
    {

        if (ENEMY_ATTACK_STATE == 0)
        {
            MovementMethods();
        }

        if (RoseInAttackPresence() && ENEMY_ATTACK_STATE == 0)
        {
            ENEMY_ATTACK_STATE = 1;
            AttackMethods();
        }
    }

    #endregion

    #region MovementMethods

    private void MovementMethods()
    {
        if (RoseInChacePresence())
        {
            enemyChasing();
        }
        else
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
            /*if (point.transform.localPosition.x <= transform.localPosition.x)
            {
             rb.velocity = new Vector2(MovementsSpeed, 0f);
             transform.localScale = new Vector2(-1, 1);
             anim.Play("Boss1_run");

            }

            if (point.transform.localPosition.x >= transform.localPosition.x)
            {
             rb.velocity = new Vector2(-MovementsSpeed, 0f);
             transform.localScale = new Vector2(1, 1);
             anim.Play("Boss1_run");
            }*/
              enemyIdle();
    }

    private void enemyChasing()
    {
        if(player.transform.localPosition.x >= transform.localPosition.x)
        {
            rb.velocity = new Vector2(MovementsSpeed, 0f);
            transform.localScale = new Vector2(-1, 1);

        }
        else if(player.transform.localPosition.x <= transform.localPosition.x)
        {
            rb.velocity = new Vector2(-MovementsSpeed, 0f);
            transform.localScale = new Vector2(1, 1);
        }

        anim.Play("Boss1_run");
    }


    #endregion

    #region AttackMethods
    private void AttackMethods()
    {
        enemyAttack();
    }

    private void enemyAttack()
    {
        anim.Play("Boss1_attack1");
    }

    public void enemyAttackEnd()
    {
        ENEMY_ATTACK_STATE = 0;
    }
    #endregion


}
