using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class LightTriiger1 : MonoBehaviour
{
    // Start is called before the first frame update

    private Animator animator;
    [SerializeField] UnityEvent onTriggerStay;
    [SerializeField] UnityEvent onTriggerExit;
    [SerializeField] UnityEvent onTriggerEnter;

    private void Start()
    {
        animator = GetComponent<Animator>();
        
    }

    private void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onTriggerEnter.Invoke();
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        Debug.Log("LightWasTrigger");
        onTriggerStay.Invoke();
        animator.Play("Light");
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        onTriggerExit.Invoke();

    }
}

