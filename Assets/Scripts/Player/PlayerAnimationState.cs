using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationState : MonoBehaviour
{
    #region Variables

    private string State;
    public Animator animator;
    private string CurrentState;
    private PlayerMovement MovementStorage;
    private MeleeAttackManager CombatStorage;

    private bool PLAYER_IDLE;
    private bool PLAYER_RUN;
    private bool PLAYER_JUMP;
    private bool PLAYER_FALL;
    private bool PLAYER_ATTACKL;
    private bool PLAYER_ATTACKLAIR;
    private bool PLAYER_UPWARDATTACK;
    private bool PLAYER_DOWNWARDATTACK;
    private bool PLAYER_FORWARDATTACK;
    private bool PLAYER_FORWARDATTACK_RUN;
    #endregion

    #region MainMethods
    void Start()
    {
        animator = GetComponent<Animator>();
        MovementStorage = GetComponent<PlayerMovement>();
        CombatStorage = GetComponent<MeleeAttackManager>();
    }

    void Update()
    {
        Debug.Log(CurrentState);
        StateBox();
        StateMachine();
    }
    #endregion

    #region AnimationMethods

    void StateBox()
    {
        PLAYER_IDLE = MovementStorage.isIdle;
        PLAYER_RUN = MovementStorage.isRunning;
        PLAYER_FALL = MovementStorage.isFalling;
        PLAYER_JUMP = MovementStorage.isJumping;
        PLAYER_UPWARDATTACK = CombatStorage.isUpwardAttack;
        PLAYER_DOWNWARDATTACK = CombatStorage.isDownwardAttack;
        PLAYER_FORWARDATTACK = CombatStorage.isForwardAttack;
        PLAYER_FORWARDATTACK_RUN = CombatStorage.isForwardAttackRun;

}
    void StateMachine()
    {
        #region MovementAnimations
        if (PLAYER_IDLE)
        {
            animator.Play("Player_Idle");
            CurrentState = ("Idle");
            return;
        }

        if (PLAYER_RUN && !PLAYER_FORWARDATTACK_RUN)
        {
            animator.Play("Player_Running");
            CurrentState = ("Run");
            return;
        }

        if (PLAYER_JUMP)
        {
            animator.Play("Player_Jump");
            CurrentState = ("Jump");
            return;
        }

        if (PLAYER_FALL)
        {
            animator.Play("Player_Fall");
            CurrentState = ("Fall");
            return;
        }
        #endregion

        #region AttackAnimations

        if (PLAYER_UPWARDATTACK)
        {
            CurrentState = ("UpwardAttack");
            return;

        }
        if (PLAYER_DOWNWARDATTACK)
        {
            CurrentState = ("DownwardAttack");
            return;

        }
        if (PLAYER_FORWARDATTACK)
        {
            CurrentState = ("ForwardAttack");
            animator.Play("Forward_Attack");
            return;
        }
        if (PLAYER_FORWARDATTACK_RUN)
        {
            CurrentState = ("Attack_Forward_Run");
            animator.Play("Attack_Forward_Run");
            return;
        }
        #endregion
    }

    #endregion
}
