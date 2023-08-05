using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NoLight : MonoBehaviour
{

    [SerializeField] Light2D player;
    // Start is called before the first frame update

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player.enabled = false;
    }

}
