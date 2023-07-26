using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header ("Player SFX")]
    [field: SerializeField] public EventReference playerFootsteps { get; private set; }
    //[field: Header("Coinf SFX")]
    //[field: SerializeField] public EventReferenece coinCollected { get; private set; }
    
    
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
