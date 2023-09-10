using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using TMPro;
using UnityEngine.SceneManagement;

public class BonfireManager : MonoBehaviour, IDataPersistence
{

    [SerializeField] private int BonfireId;
    private Animator animator;
    [SerializeField] private string playerRespownBonfireScene;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void LoadData(GameData data)
    {

    }

    public void SaveData(ref GameData data)
    {
        data.playerBonfire = this.BonfireId;
        data.playerRespawnScene = playerRespownBonfireScene;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.tag.Equals("Player"))
        {

            animator.Play("BonfireOpen");

            var player = collision.GetComponent<PlayerGodController>();

            if (player.playerControls.PlayerActions.Interaction.WasPressedThisFrame())
            {
                player.playerCurrentHp = player.playerMaxHp;
                Debug.Log("Heal!");
                DataPersistenceManager.Instance.SaveGame();
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        animator.Play("BonfireExit");
    }
}
