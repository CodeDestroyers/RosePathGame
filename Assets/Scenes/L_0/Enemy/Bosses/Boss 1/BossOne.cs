using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossOne : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private int damage = 20;
    private float cooldownTImer = Mathf.Infinity;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;

    private Animator anim;
    private int playerHealt;
    [SerializeField]  private bool attackState;

    [Header("Patrol Points")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;

    [Header("Movement parameters")]
    [SerializeField] private float speed;
    private Vector3 initScale;
    private bool movingLeft;

    [Header("Idle Behaviour")]
    [SerializeField] private float idleDuration;
    private float idleTimer;

    [Header("Enemy Animator")]
    [SerializeField] private bool attackStateBoss;
    [SerializeField] private bool bossRun;
    [SerializeField] private bool bossIdle;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        initScale = enemy.localScale;
    }

    private void Start()
    {
        attackState = false;
        attackStateBoss = false;
    }

    private void Update()
    {
        cooldownTImer += Time.deltaTime;

        if (PlayerInSight())
        {
            if (cooldownTImer >= attackCooldown)
            {
                cooldownTImer = 0;
            }
        }


        bossMove();
        DirectionChange();
        animateBoss();
        bossAttack();


    }

    private void OnDisable()
    {
        anim.Play("Boss1_idle");
    }

    public bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z), 0, Vector2.left, 0, playerLayer);

        if(hit.collider != null && !attackState && !attackStateBoss)
        {
            playerHealt = hit.transform.GetComponent<PlayerMovement>().playerHP;
            //bossIdle = false;
            //bossRun = false;
        }
        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    //Invoke on event in animator
    private void DamagePlayer()
    {
        if (PlayerInSight())
        {
            playerHealt -= damage;
        }
    }

    private void bossAttack()
    {
        if (PlayerInSight())
        {
            attackState = true;
            attackStateBoss = true;
            bossRun = false;
            bossIdle = false;
        }
    }



    public void attackStop()
    {
        attackState = false;
        attackStateBoss = false;
    }

    private void bossMove()
    {
        if (movingLeft && !attackState)
        {
            if (enemy.position.x >= leftEdge.position.x)
            {
                MoveInDirection(-1);
                bossIdle = false;
                bossRun = true;
            }

            else
            {
                DirectionChange();
                bossIdle = true;
                bossRun = false;
            }

        }
        else if (!attackState)
        {
            if (enemy.position.x <= rightEdge.position.x)
            {
                MoveInDirection(1);
                bossIdle = false;
                bossRun = true;
            }

            else
            {
                DirectionChange();
                bossIdle = true;
                bossRun = false;
            }
        }
    }

    private void DirectionChange()
    {
            idleTimer += Time.deltaTime;

            if (idleTimer > idleDuration)
            {
                movingLeft = !movingLeft;
            }
    }

    private void MoveInDirection(int _direction)
    {
            idleTimer = 0;

            //Make enemy face direction
        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction,
                initScale.y, initScale.z);

        //Move in that direction
        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed,
            enemy.position.y, enemy.position.z);
    }

    private void animateBoss()
    {
        if (bossIdle)
        {
            anim.Play("Boss1_idle");
        }

        if (bossRun)
        {
            anim.Play("Boss1_run");
        }
        if (attackStateBoss)
        {
            anim.Play("Boss1_attack1");
        }
    }
}
