using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LampTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;

    void OnTriggerEnter2D(Collider2D other)
    {
        onTriggerEnter.Invoke();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        onTriggerEnter.Invoke();
    }

}
