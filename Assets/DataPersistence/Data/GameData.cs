using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int playerMaxHP;
    public int playerCurrentHp;
    public int enemyRespawn;
    public int playerBonfire;
    public string playerRespawnScene;

    public SerializableDictionary<string, bool> collectedItems;

    public GameData()
    {
        playerRespawnScene = "L0_S0";
        playerBonfire = 0;
        enemyRespawn = 0;
        playerMaxHP = 200;
        playerCurrentHp = 200;
        collectedItems = new SerializableDictionary<string, bool>();

    }
}
 