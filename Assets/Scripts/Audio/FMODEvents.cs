using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header ("Player SFX")]
    [field: SerializeField] public EventReference playerFootsteps { get; private set; }
    [field: SerializeField] public EventReference playerJump { get; private set; }

    [field: SerializeField] public EventReference baseAttack { get; private set; }

    [field: SerializeField] public EventReference playerTakeHit { get; private set; }

    [field: SerializeField] public EventReference playerHitEnemy { get; private set; }

    [field: SerializeField] public EventReference playerHitWall { get; private set; }

    [field: SerializeField] public EventReference playerHitEnviroment { get; private set; }

    [field: Header("L0")] 
    [field: SerializeField] public EventReference WindBase { get; private set; }

    [field: SerializeField] public EventReference objectDestroy1 { get; private set; }



    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Найдено два и более ФМОД-эвента на одной сцене");
        }
        instance = this;
    }
}
