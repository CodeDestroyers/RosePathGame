using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using TMPro;

public class BonfireManager : MonoBehaviour, IDataPersistence
{

    [SerializeField] private string BonfireId;
    [SerializeField] private TextMeshPro announce;
    private bool playerInPresence;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInPresence)
        {
            announce.color = Color.clear;
        }
    }

    public void LoadData(GameData data)
    {
        data.playerCurrentHp = data.playerMaxHP;
    }

    public void SaveData(ref GameData data)
    {
        data.playerCurrentHp = data.playerMaxHP;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        playerInPresence = true;

        if (collision.gameObject.tag.Equals("Player"))
        {
            var player = collision.GetComponent<PlayerGodController>();
            announce.color = Color.Lerp(Color.clear, Color.grey, 0.5f);

            if (player.playerControls.PlayerActions.Interaction.WasPressedThisFrame())
            {
                DataPersistenceManager.Instance.SaveGame();
                player.playerCurrentHp = player.playerMaxHp;
                Debug.Log("Heal!");
            }
        }

        else
        {
            announce.color = Color.clear;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerInPresence = false;
    }
}
