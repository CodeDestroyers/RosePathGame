using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightTriiger : MonoBehaviour
{
    // Start is called before the first frame update

    private Animator animator;
    [SerializeField] private float speed = 1f;

    private void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<PlayerMovement>() != null)
        {
            animator.Play("Light");
            GameObject temp = GameObject.Find("LightToward");
            transform.position = Vector2.MoveTowards(transform.position, temp.transform.position, speed * Time.deltaTime);

        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
    }
}
