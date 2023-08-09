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
    private BossMovement enemyPatrol;
    public bool attackState;

    private void Awake()
    {
        enemyPatrol = GetComponent<BossMovement>();
        anim = GetComponent<Animator>();
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


        bossAttackState();

        Debug.Log(attackState);


    }

    public bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z), 0, Vector2.left, 0, playerLayer);

        if(hit.collider != null)
        {
            playerHealt = hit.transform.GetComponent<PlayerMovement>().playerHP;
            anim.Play("Boss1_attack1");
        }

        return hit.collider != null;
    }

    private void bossAttackState()
    {
        if (PlayerInSight())
        {
            attackState = true;
        }
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

    private void attackStop()
    {
        attackState = false;
    }
}
