using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using TMPro;

public class ItemManager : MonoBehaviour, IDataPersistence
{

    [SerializeField] private string id;
    [SerializeField] private TextMeshPro announce;
    private bool playerInPresence;

    private bool collected;
    // Start is called before the first frame update
    void Start()
    {
        collected = false;   
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
        data.collectedItems.TryGetValue(id, out collected);
        {
            if (collected)
            {
              gameObject.SetActive(false);
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.collectedItems.ContainsKey(id))
        {
            data.collectedItems.Remove(id);
        }
        data.collectedItems.Add(id, collected);

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        playerInPresence = true;

        if (collision.gameObject.tag.Equals("Player"))
        {
            var player = collision.GetComponent<PlayerGodController>();
            player.playerCurrentHp = 200;
            announce.color = Color.Lerp(Color.clear, Color.grey, 0.5f);
            

            if (player.playerControls.PlayerActions.Interaction.WasPressedThisFrame())
            {
                collected = true;
                gameObject.SetActive(false);
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
