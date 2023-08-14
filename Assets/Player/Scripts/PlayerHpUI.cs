using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class PlayerHpUI : MonoBehaviour
{

    private Image hpBar;
    public Sprite[] hpBar200;
    public PlayerGodController player;

    // Start is called before the first frame update
    void Awake()
    {
        hpBar = GetComponent<Image>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerGodController>();
    }

    // Update is called once per frame
    void Update()
    {
        MaxHpChecker();
    }

    private void MaxHpChecker()
    {
        if (player.playerMaxHp == 200)
        {
            HpUpdater200();
        }
    }

    private void HpUpdater200()
    {
        if (player.playerCurrentHp >= 151 && player.playerCurrentHp <= 200)
        {
            hpBar.sprite = hpBar200[0];
        }

        if (player.playerCurrentHp >= 101 && player.playerCurrentHp <= 150)
        {
            hpBar.sprite = hpBar200[1];
        }

        if (player.playerCurrentHp >= 51 && player.playerCurrentHp <= 100)
        {
            hpBar.sprite = hpBar200[2];
        }

        if (player.playerCurrentHp >= 1 && player.playerCurrentHp <= 50)
        {
            hpBar.sprite = hpBar200[3];
        }

        if (player.playerCurrentHp <= 0)
        {
            hpBar.sprite = hpBar200[4];
        }
    }
}
