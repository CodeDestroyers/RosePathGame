using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class BossMovement : MonoBehaviour
{
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
    [SerializeField] private Animator anim;

    public BossOne state;
    private bool attackStateBoss;

    private void Awake()
    {
        initScale = enemy.localScale;
    }

    private void Start()
    {
        state = GetComponent<BossOne>();
        //FMODUnity.RuntimeManager.PlayOneShot("event:/EnemySounds/L0_S0/Bosses/Boss1/Boss1_Footstep");
    }
    private void OnDisable()
    {
        anim.Play("Boss1_idle");
    }   

    private void Update()
    {

        if (movingLeft)
        {
            if (enemy.position.x >= leftEdge.position.x)
                MoveInDirection(-1);
            else
                DirectionChange();
        }
        else
        {
            if (enemy.position.x <= rightEdge.position.x)
                MoveInDirection(1);
            else
                DirectionChange();
        }
    }

    private void DirectionChange()
    {
        if (!attackStateBoss || !state.PlayerInSight())
        {
            anim.Play("Boss1_idle");
            idleTimer += Time.deltaTime;

            if (idleTimer > idleDuration)
                movingLeft = !movingLeft;
        }
    }

    private void MoveInDirection(int _direction)
    {
        if (!attackStateBoss || !state.PlayerInSight())
        {
            idleTimer = 0;
            anim.Play("Boss1_run");

            //Make enemy face direction
            enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction,
                initScale.y, initScale.z);

            //Move in that direction
            enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed,
                enemy.position.y, enemy.position.z);
        }
    }
}
