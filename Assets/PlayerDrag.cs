using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDrag : MonoBehaviour
{
    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        onTriggerEnter.Invoke();
    }
}
