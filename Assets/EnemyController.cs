using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private int damageAmount = 15;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Checks to see if the GameObject the MeleeWeapon is colliding with has an EnemyHealth script
        if (collision.GetComponent<PlayerGodController>())
        {
            //Method that checks to see what force can be applied to the player when melee attacking
            DamagePlayer(collision.GetComponent<PlayerGodController>());
            Debug.Log("Player was hit!");
        }
    }

    private void DamagePlayer(PlayerGodController objHealth)
    {
        objHealth.Damage(damageAmount);
    }
}
