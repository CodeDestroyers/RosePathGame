using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPointRight : MonoBehaviour
{
    SimpleEnemyController enenemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Enemy"))
        {
            enenemy = collision.GetComponent<SimpleEnemyController>();
            enenemy.goLeft = true;
            enenemy.goRight = false;
        }
    }
}
