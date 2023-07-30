using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    #region Variables

    public PlayerControls playerControls;
    public bool isAttackL;
    private PlayerMovement player;

    #endregion

    #region MainMethods
    void Start()
    {
       player = GetComponent<PlayerMovement>();
       OnEnable();
       AttackLEND();
    }

    void Update()
    {
        AttackL();
    }
    private void Awake()
    {
        playerControls = new PlayerControls();

    }
    private void OnEnable()
    {
        playerControls.Enable();
    }

    #endregion

    #region AttackMethods

    private void AttackL()
    {
        if (playerControls.PlayerActions.AttackL.WasPerformedThisFrame())
        {
            isAttackL = true;
            player.ZeroState = 1;
        }
    }
    private void AttackLEND()
    {
        isAttackL = false;
        player.ZeroState = 0;
    }



    #endregion

    #region SkillMethods

    #endregion
}

