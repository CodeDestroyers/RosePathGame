using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UiPlayerManager : MonoBehaviour
{

    private PlayerControls playerControls;
    public GameObject escMenue;
    public static bool gameIsPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        OnEnable();
    }

    // Update is called once per frame
    void Update()
    {
        EscapeCall();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Awake()    
    {
        playerControls = new PlayerControls();

    }

    private void EscapeCall()
    {
        if (playerControls.PlayerActions.EscMenue.WasPerformedThisFrame() && !gameIsPaused)
        {
            escMenue.SetActive(true);
            gameIsPaused = true;
            Time.timeScale = 0f;
        }
        else if (playerControls.PlayerActions.EscMenue.WasPerformedThisFrame() && gameIsPaused)
        {
            escMenue.SetActive(false);
            gameIsPaused = false;
            Time.timeScale = 1f;
        }
    }
    public void TimeResume()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
    }
}
