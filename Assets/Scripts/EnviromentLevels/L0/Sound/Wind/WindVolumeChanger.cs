using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindVolumeChanger : MonoBehaviour
{
    [Header("Parameter Change")]

    [SerializeField] string paramterName;

    [SerializeField] float paramterValue;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            AudioManager.instance.SetAmbientParmeter(paramterName, paramterValue);
        }
    }
}
