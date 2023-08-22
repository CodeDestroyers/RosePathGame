using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPointLeft : MonoBehaviour
{
    SimpleEnemyController enenemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Enemy"))
        {
            enenemy = collision.GetComponent<SimpleEnemyController>();
            enenemy.goLeft = false;
            enenemy.goRight = true;
        }
    }
}
