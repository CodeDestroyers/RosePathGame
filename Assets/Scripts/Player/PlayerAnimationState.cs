using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationState : MonoBehaviour
{
    #region Variables

    private string State;
    public Animator animator;
    private string CurrentState;
    private PlayerMovement StateStorage;

    private bool PLAYER_IDLE;
    private bool PLAYER_RUN;
    private bool PLAYER_JUMP;
    private bool PLAYER_FALL;
    private bool PLAYER_ATTACKL;
    private bool PLAYER_ATTACKLAIR;
    #endregion

    #region MainMethods
    void Start()
    {
        animator = GetComponent<Animator>();
        StateStorage = GetComponent<PlayerMovement>();

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
        var StateSwitch = GetComponent<PlayerMovement>();
        PLAYER_IDLE = StateSwitch.isIdle;
        PLAYER_RUN = StateSwitch.isRunning;
        PLAYER_FALL = StateSwitch.isFalling;
        PLAYER_JUMP = StateSwitch.isJumping;

    }
    void StateMachine()
    {
        if (PLAYER_IDLE)
        {
            animator.Play("Player_Idle");
            CurrentState = ("Idle");
            return;
        }

        if (PLAYER_RUN)
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

    }

    #endregion
}
